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

        public ContractionVM(Settings settings, string longForm, FormattedString shortForm, FormattedString allBrailleShortForm)
        {
            Settings = settings;
            LongForm = longForm;
            ShortForm = shortForm;
            AllBrailleShortForm = allBrailleShortForm;
        }
    }
}
