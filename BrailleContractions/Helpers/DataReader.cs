using BrailleContractions.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BrailleContractions.Helpers
{
    public class DataReader
    {
        /// <summary>
        /// Invoked when finished reading.
        /// </summary>
        private event Action<ContractionVM[]> OnReadComplete;

        private readonly object _lock = new object();
        private bool _done;

        /// <summary>
        /// All of the contractions parsed from the data file.
        /// </summary>
        private ContractionVM[] _data;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Passed to each new contraction object.</param>
        public DataReader(Settings settings)
        {
            Task.Run(() =>
            {
                _data = ReadUebContractions(settings).ToArray();

                lock (_lock)
                {
                    OnReadComplete?.Invoke(_data);
                    _done = true;
                }
            });
        }

        /// <summary>
        /// Performs an action after this class is done reading the data.
        /// If it's already done, the action is invoked immediately.
        /// </summary>
        /// <param name="whenDone">The action.</param>
        public void WhenDone(Action<ContractionVM[]> whenDone)
        {
            lock (_lock)
            {
                if (_done)
                {
                    whenDone.Invoke(_data);
                }
                else
                {
                    OnReadComplete += whenDone;
                }
            }
        }

        /// <summary>
        /// Reads the UEB contraction data file.
        /// </summary>
        /// <param name="settings">Passed to each new contraction object.</param>
        /// <returns>A list of contractions.</returns>
        private static IEnumerable<ContractionVM> ReadUebContractions(Settings settings)
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
                        var longFormBraille = new StringBuilder();
                        string symbol = null;
                        var shortForm = new FormattedString();
                        var shortFormBraille = new FormattedString();

                        // The first 26 entries should be letters. Add them to the dictionary as they appear.
                        if (letters.Count < 26 && longForm.Length > 0 && split[1].Length > 0)
                        {
                            letters.Add(longForm[0], split[1][0]);
                        }

                        // For each character in the long (uncontracted) form
                        foreach (char character in longForm)
                        {
                            // Replace letters a-z with Braille characters
                            if (!letters.TryGetValue(character, out char brailleChar))
                            {
                                // This is the character to use in the all-Braille form. Use the original character by default.
                                brailleChar = character;
                            }

                            longFormBraille.Append(brailleChar);
                        }

                        // For each character in the short (contracted) form
                        foreach (char character in split[1])
                        {
                            if (!letters.TryGetValue(character, out char brailleChar))
                            {
                                brailleChar = character;
                            }

                            // Append to the regular and all-Braille short forms
                            AppendCharacter(shortForm, character);
                            AppendCharacter(shortFormBraille, brailleChar);
                        }

                        // If there is a third item on this line, it's a special symbol such as "$".
                        if (split.Length > 2)
                        {
                            symbol = split[2];
                        }

                        yield return new ContractionVM(settings, longForm, longFormBraille.ToString(), symbol, shortForm, shortFormBraille);
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
            var lastSpan = formattedString.Spans.LastOrDefault();
            bool isLastCharBraille = lastSpan?.Text.Last().IsBraille() ?? false;
            bool isNewCharBraille = newChar.IsBraille();

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
                    span.FontFamily = "JBraille";
                }

                formattedString.Spans.Add(span);
            }
        }
    }
}
