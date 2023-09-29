using BrailleContractions.Services;
using Xamarin.Forms;

namespace BrailleContractions.ViewModels
{
    /// <summary>
    /// Contains global settings that views can bind to.
    /// A single instance should be created at startup and passed to objects that depend on settings.
    /// </summary>
    public class Settings : BaseVM
    {
        /// <summary>
        /// Gets the platform-specific implementation of <see cref="ISystemService"/>.
        /// </summary>
        public ISystemService SystemService { get; } = DependencyService.Get<ISystemService>();

        private bool _displayShortFormInBraille;

        /// <summary>
        /// If true, Braille characters are shown instead of letters a-z.
        /// </summary>
        public bool DisplayShortFormInBraille
        {
            get => _displayShortFormInBraille;
            set => SetProperty(ref _displayShortFormInBraille, value);
        }

        private int _dotSize;

        /// <summary>
        /// The size of the dots in the Braille input cells.
        /// Can be changed in response to screen size and orientation.
        /// </summary>
        public int DotSize
        {
            get => _dotSize;
            set => SetProperty(ref _dotSize, value);
        }
    }
}
