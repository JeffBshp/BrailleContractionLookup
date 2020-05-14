using BrailleContractions.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace BrailleContractions.Helpers
{
    internal static class DataReader
    {
        private static readonly string JBraille;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static DataReader()
        {
            // Determine the path to the Braille font
            switch (Device.RuntimePlatform)
            {
                case "Android":
                    JBraille = "fonts/JBraille.ttf#JBraille";
                    break;
                case "iOS":
                    // TODO
                    goto default;
                default:
                    JBraille = string.Empty;
                    break;
            }
        }

        /// <summary>
        /// Reads the UEB contraction data file.
        /// </summary>
        /// <returns>A list of contractions.</returns>
        public static IEnumerable<ContractionVM> ReadUebContractions(Settings settings)
        {
            var letters = new Dictionary<char, char>();
            var assembly = Assembly.GetAssembly(typeof(App));

            using (var stream = assembly.GetManifestResourceStream("BrailleContractions.Resources.data.txt"))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var split = reader.ReadLine().Split('|');

                    if (split.Length >= 2)
                    {
                        string longForm = split[0];
                        var shortForm = new FormattedString();
                        var braille = new FormattedString();

                        // The first 26 entries should be letters. Add them to the dictionary as they appear.
                        if (letters.Count < 26 && longForm.Length > 0 && split[1].Length > 0)
                        {
                            letters.Add(longForm[0], split[1][0]);
                        }

                        // For each character in the short (contracted) form
                        foreach (char character in split[1])
                        {
                            // Replace letters a-z with Braille characters
                            if (!letters.TryGetValue(character, out char brailleChar))
                            {
                                // This is the character to use in the all-Braille form. Use the original character by default.
                                brailleChar = character;
                            }

                            // Append to the regular and all-Braille short forms
                            AppendCharacter(shortForm, character);
                            AppendCharacter(braille, brailleChar);
                        }

                        // If there is a third item on this line, it's a special symbol. Just append it to the long form.
                        // TODO: Remove this logic and instead combine these items in the data file?
                        if (split.Length > 2)
                        {
                            longForm += ' ' + split[2];
                        }

                        yield return new ContractionVM(settings, longForm, shortForm, braille);
                    }
                }
            }
        }

        /// <summary>
        /// Appends a new character to the string.
        /// Braille characters are grouped into spans with the custom Braille font.
        /// Non-Braille characters are grouped into spans with the default font.
        /// </summary>
        /// <param name="formattedString">The string with spans of Braille and non-Braille characters.</param>
        /// <param name="newChar">The character to append.</param>
        private static void AppendCharacter(FormattedString formattedString, char newChar)
        {
            bool isBraille(char? c) => c.HasValue && c >= 0x2800 && c <= 0x28FF;

            var lastSpan = formattedString.Spans.LastOrDefault();
            bool isLastCharBraille = isBraille(lastSpan?.Text.Last());
            bool isNewCharBraille = isBraille(newChar);

            if (lastSpan != null && isLastCharBraille == isNewCharBraille)
            {
                lastSpan.Text += newChar;
            }
            else
            {
                var span = new Span
                {
                    Text = new string(newChar, 1),
                    FontSize = 24
                };

                if (isNewCharBraille)
                {
                    span.FontFamily = JBraille;
                }

                formattedString.Spans.Add(span);
            }
        }
    }
}
