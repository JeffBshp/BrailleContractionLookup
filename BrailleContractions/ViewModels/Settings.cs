namespace BrailleContractions.ViewModels
{
    /// <summary>
    /// Platforms provide a global font scaling feature for accessibility.
    /// The platform-specific values are converted to this enum, making them easier for this app to handle.
    /// The integer values are used as the RowHeight of the ListView (see <see cref="LookupPageVM"/>).
    /// </summary>
    public enum FontScale
    {
        Small = 40,
        Medium = 52,
        Large = 64,
        ExtraLarge = 76
    }

    /// <summary>
    /// Contains global settings that views can bind to.
    /// A single instance should be created at startup and passed to objects that depend on settings.
    /// </summary>
    public abstract class Settings : BaseVM
    {
        /// <summary>
        /// Platform-specific app version.
        /// </summary>
        public abstract string AppVersion { get; }

        /// <summary>
        /// Platform-specific image name for the backspace button icon.
        /// </summary>
        public abstract string BackspaceIcon { get; }

        /// <summary>
        /// Platform-specific image name for the clear button icon.
        /// </summary>
        public abstract string ClearIcon { get; }

        /// <summary>
        /// Platform-specific image name for the info button icon.
        /// </summary>
        public abstract string InfoIcon { get; }

        /// <summary>
        /// Represents the device's accessibility font scale setting.
        /// The platform-specific value is converted to the <see cref="ViewModels.FontScale"/> enum.
        /// </summary>
        public FontScale FontScale
        {
            get => _fontScale;
            set => SetProperty(ref _fontScale, value);
        }
        private FontScale _fontScale;

        /// <summary>
        /// Font size for Labels throughout the app.
        /// </summary>
        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }
        private double _fontSize;

        /// <summary>
        /// If true, Braille characters are shown instead of letters a-z.
        /// </summary>
        public bool DisplayShortFormInBraille
        {
            get => _displayShortFormInBraille;
            set => SetProperty(ref _displayShortFormInBraille, value);
        }
        private bool _displayShortFormInBraille;

        /// <summary>
        /// The size of the dots in the Braille input cells.
        /// Can be changed in response to screen size and orientation.
        /// </summary>
        public int DotSize
        {
            get => _dotSize;
            set => SetProperty(ref _dotSize, value);
        }
        private int _dotSize;
    }
}
