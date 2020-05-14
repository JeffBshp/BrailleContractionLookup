using BrailleContractions.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace BrailleContractions.ViewModels
{
    public class LookupPageVM : BaseVM
    {
        private string _text = string.Empty;

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public BrailleInputCellVM[] Cells { get; } = new BrailleInputCellVM[4];

        private bool _cellsVisible;

        public bool CellsVisible
        {
            get => _cellsVisible;
            set => SetProperty(ref _cellsVisible, value);
        }

        public Settings Settings { get; }

        /// <summary>
        /// The RowHeight to set on the ListView.
        /// </summary>
        public double RowHeight { get; }

        private readonly IReadOnlyList<ContractionVM> _allContractions;

        /// <summary>
        /// Contractions that are currently visible in the ListView.
        /// </summary>
        public List<ContractionVM> Contractions { get; private set; }

        /// <summary>
        /// Set to true when programmatically changing the state of the Braille input cells.
        /// This makes it possible to differentiate such changes from user input.
        /// A TapGestureRecognizer is not ideal because tapping is not the only way a user can toggle a dot,
        /// especially in an app that is supposed to be accessible to the visually impaired.
        /// </summary>
        private bool _updatingCells;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">App settings.</param>
        public LookupPageVM(Settings settings, IEnumerable<ContractionVM> contractions)
        {
            Settings = settings;
            _allContractions = contractions.ToList();
            Contractions = new List<ContractionVM>(_allContractions);

            // Adjust the row height to hopefully accommodate the device's font scale setting.
            // On Android the possible values are 0.85, 1.00, 1.15, 1.30
            double fontScale = DependencyService.Get<ISystemService>().FontScale;
            RowHeight = fontScale < 1 ? 40
                : fontScale < 1.1 ? 48
                : fontScale < 1.2 ? 56
                : 64;

            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new BrailleInputCellVM(settings);
                Cells[i].PropertyChanged += CellPropertyChanged;
            }
        }

        /// <summary>
        /// Called when the user edits the search text.
        /// </summary>
        /// <param name="newText">The text after the edit.</param>
        public void TextChanged(string newText)
        {
            // Set this flag so we can change the state of the Braille input cells without triggering another text change
            _updatingCells = true;

            // If the user entered only spaces and digits, treat these as dot numbers and update the cells accordingly
            if (newText.All(x => x == ' ' || char.IsDigit(x)))
            {
                var cellStates = newText.Split(' ');

                for (int i = 0; i < Cells.Length; i++)
                {
                    int dots = 0;

                    if (i < cellStates.Length)
                    {
                        // Select the individual digits representing Braille dot numbers.
                        // We split on space, so all characters are digits here.
                        var dotNumbers = cellStates[i].Select(char.GetNumericValue).Where(x => x >= 1 && x <= 6);
                        // Convert to a bitfield
                        dots = dotNumbers.Aggregate(0, (x, y) => x | (int)Math.Pow(2, y - 1));
                    }

                    Cells[i].Dots = dots;
                }
            }
            // Otherwise clear all cells and assume they want to do a text-based search
            else
            {
                foreach (var cell in Cells)
                {
                    cell.Dots = 0;
                }
            }

            _updatingCells = false;

            Search();
        }

        /// <summary>
        /// Clears the last non-empty cell or deletes one character from the text box.
        /// </summary>
        public void Backspace()
        {
            bool allCellsEmpty = true;
            string text = Text;

            // Work backwards until we find a non-empty cell to clear
            for (int i = Cells.Length - 1; i >= 0; i--)
            {
                if (Cells[i].Dots != 0)
                {
                    // CellPropertyChanged will be called
                    Cells[i].Dots = 0;
                    allCellsEmpty = false;
                    break;
                }
            }

            // If all cells are empty, delete a character from the text box instead
            if (allCellsEmpty && text.Length > 0)
            {
                Text = text.Substring(0, text.Length - 1);
                // CellPropertyChanged was not called, so perform the search here
                Search();
            }
        }

        /// <summary>
        /// Clears the text box and all Braille input cells.
        /// </summary>
        public void Clear()
        {
            // Set this flag to avoid calling Search until we finish the loop
            _updatingCells = true;
            foreach (var cell in Cells)
            {
                cell.Dots = 0;
            }
            _updatingCells = false;

            Text = string.Empty;

            Search();
        }

        /// <summary>
        /// Updates the text box and performs a search when a Braille input cell changes state.
        /// </summary>
        private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Ignore when we are programmatically updating cells.
            // The PropertyName check just ensures that we only handle each change once.
            if (!_updatingCells && (e.PropertyName == nameof(BrailleInputCellVM.Dots) || string.IsNullOrEmpty(e.PropertyName)))
            {
                var text = new StringBuilder();
                bool anyDots = false;

                // Work backwards
                for (int i = Cells.Length - 1; i >= 0; i--)
                {
                    // Get a string that contains the dot numbers of the current cell
                    string cellState = string.Join(string.Empty, Enumerable.Range(1, 6)
                        .Where(x => (Cells[i].Dots & (int)Math.Pow(2, x - 1)) != 0));

                    // Skip empty cells until we find the first (from the end) non-empty cell.
                    // Then insert all cells whether they are empty or not.
                    // So if Cells[1] has dots 1 & 2 filled while the other cells are empty,
                    // then the final string will be " 12" (note the leading space).
                    if (anyDots || cellState.Length > 0)
                    {
                        anyDots = true;
                        text.Insert(0, cellState);
                        text.Insert(0, ' ');
                    }
                }

                if (text.Length > 0)
                {
                    // Remove the initial space character
                    text.Remove(0, 1);
                }

                Text = text.ToString();
                Search();
            }
        }

        /// <summary>
        /// Filters the ListView to show only items that match the current text or Braille cell input.
        /// </summary>
        private void Search()
        {
            string query = Cells.Any(x => x.Dots != 0)
                    ? new string(Cells.Where(x => x.Dots != 0).Select(x => x.Char).ToArray())
                    : Text;

            var results = query.Length == 0
                ? _allContractions.AsEnumerable()
                : _allContractions.Where(x =>
                    x.LongForm.Contains(query) ||
                    x.ShortForm.ToString().Contains(query) ||
                    x.AllBrailleShortForm.ToString().Contains(query));

            Contractions = new List<ContractionVM>(results);
            OnPropertyChanged(nameof(Contractions));
        }
    }
}
