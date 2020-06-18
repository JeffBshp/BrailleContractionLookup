using BrailleContractions.Helpers;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BrailleContractions.ViewModels
{
    /// <summary>
    /// ViewModel for an entry in the list of contractions.
    /// </summary>
    public class ContractionVM : BaseVM
    {
        /// <summary>
        /// <see cref="Settings.DisplayShortFormInBraille"/> determines which short form is displayed.
        /// </summary>
        public Settings Settings { get; }

        /// <summary>
        /// The uncontracted form.
        /// </summary>
        public string LongForm { get; }

        /// <summary>
        /// The contracted form.
        /// </summary>
        public FormattedString ShortForm { get; }

        /// <summary>
        /// The contracted form, but letters a-z are replaced with Braille characters.
        /// </summary>
        public FormattedString AllBrailleShortForm { get; }

        public string AutomationHelpText { get; }

        public ContractionVM(Settings settings, string longForm, FormattedString shortForm, FormattedString allBrailleShortForm)
        {
            Settings = settings;
            LongForm = longForm;
            ShortForm = shortForm;
            AllBrailleShortForm = allBrailleShortForm;
            AutomationHelpText = string.Join(" ", allBrailleShortForm.ToString().Select(BrailleCharacterDescription));
        }

        /// <summary>
        /// Returns a string that lists the dots in a Braille character.
        /// For example, the letter "b" is described as "(Dots 1, 2)".
        /// This is necessary because Google TalkBack does not read literal Braille characters.
        /// </summary>
        /// <param name="c">The character to describe.</param>
        private static string BrailleCharacterDescription(char c)
        {
            string description;

            if (c.IsBraille())
            {
                // Might as well handle all 8 possible dots even though only dots 1-6 are used in this app
                const int numberOfDots = 8;
                var dotNumbers = new List<int>(numberOfDots);

                for (int i = 0; i < numberOfDots; i++)
                {
                    int mask = 1 << i;

                    if ((c & mask) == mask)
                    {
                        dotNumbers.Add(i + 1);
                    }
                }

                if (dotNumbers.Count == 0)
                {
                    description = "Space";
                }
                else
                {
                    string s = dotNumbers.Count > 1 ? "s" : string.Empty;
                    description = $"(Dot{s} {string.Join(", ", dotNumbers)})";
                }
            }
            else
            {
                // Not Braille, so just return the character as is
                description = new string(c, 1);
            }

            return description;
        }
    }
}
