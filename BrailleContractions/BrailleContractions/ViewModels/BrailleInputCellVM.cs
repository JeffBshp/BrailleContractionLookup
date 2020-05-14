using System.Runtime.CompilerServices;

namespace BrailleContractions.ViewModels
{
    /// <summary>
    /// Manages the state of a Braille cell UI element with toggleable dots.
    /// </summary>
    public class BrailleInputCellVM : BaseVM
    {
        private int _dots;

        /// <summary>
        /// A bitfield where the six least significant, rightmost bits represent the six Braille cell dots.
        /// </summary>
        public int Dots
        {
            get => _dots;
            set
            {
                if (_dots != value)
                {
                    _dots = value;
                    // Rather than check which dots have changed, simply indicate that all properties have changed
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        /// <summary>
        /// The ASCII character for the Braille cell that this view model currently represents.
        /// Values range from 0x2800 to 0x283F.
        /// </summary>
        public char Char => (char) (0x2800 | _dots);

        #region Dots

        /// <summary>
        /// The top-left dot.
        /// </summary>
        public bool Dot1
        {
            get => GetBit(0);
            set => SetBit(0, value);
        }

        /// <summary>
        /// The middle-left dot.
        /// </summary>
        public bool Dot2
        {
            get => GetBit(1);
            set => SetBit(1, value);
        }

        /// <summary>
        /// The bottom-left dot.
        /// </summary>
        public bool Dot3
        {
            get => GetBit(2);
            set => SetBit(2, value);
        }

        /// <summary>
        /// The top-right dot.
        /// </summary>
        public bool Dot4
        {
            get => GetBit(3);
            set => SetBit(3, value);
        }

        /// <summary>
        /// The middle-right dot.
        /// </summary>
        public bool Dot5
        {
            get => GetBit(4);
            set => SetBit(4, value);
        }

        /// <summary>
        /// The bottom-right dot.
        /// </summary>
        public bool Dot6
        {
            get => GetBit(5);
            set => SetBit(5, value);
        }

        #endregion

        public Settings Settings { get; }

        public BrailleInputCellVM(Settings settings)
        {
            Settings = settings;
        }

        private bool GetBit(int bitIndex) => (_dots & (1 << bitIndex)) != 0;

        private void SetBit(int bitIndex, bool value, [CallerMemberName] string propertyName = null)
        {
            // Get the dots with the single bit flipped
            int bit = 1 << bitIndex;
            int newDots = value
                ? _dots | bit
                : _dots & (~bit);

            // Set the backing field
            SetProperty(ref _dots, newDots, () =>
            {
                // PropertyChanged will be invoked automatically for the dot indicated by propertyName (Dot1-Dot6),
                // but we also need to invoke it for other properties that use the same backing field.
                OnPropertyChanged(nameof(Dots));
                OnPropertyChanged(nameof(Char));
            }, propertyName);
        }
    }
}
