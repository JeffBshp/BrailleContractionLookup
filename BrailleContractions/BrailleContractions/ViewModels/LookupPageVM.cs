using BrailleContractions.Helpers;
using BrailleContractions.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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

        private bool _listIsRefreshing = true;

        /// <summary>
        /// Toggles the refresh animation on the ListView.
        /// </summary>
        public bool ListIsRefreshing
        {
            get => _listIsRefreshing;
            private set => SetProperty(ref _listIsRefreshing, value);
        }

        private IReadOnlyList<ContractionVM> _allContractions = new List<ContractionVM>(0);

        private List<ContractionVM> _contractions = new List<ContractionVM>(0);

        /// <summary>
        /// Contractions that are currently visible in the ListView.
        /// </summary>
        public List<ContractionVM> Contractions
        {
            get => _contractions;
            private set => SetProperty(ref _contractions, value);
        }

        private bool _ignoreChanges;
        private int _updateId;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">App settings.</param>
        public LookupPageVM(Settings settings)
        {
            Settings = settings;

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

            Task.Run(() =>
            {
                _allContractions = DataReader.ReadUebContractions(settings).ToList();
                // Back to the main thread to update the UI, which also ends the initial refresh animation
                Device.BeginInvokeOnMainThread(async () => await Update(Text, false, false));
            });
        }

        /// <summary>
        /// Updates the text box and performs a search when a Braille input cell changes state.
        /// </summary>
        private async void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Only respond to a change of the "Dots" property or all properties (empty string), not Dot1 through Dot6
            if (e.PropertyName == nameof(BrailleInputCellVM.Dots) || string.IsNullOrEmpty(e.PropertyName))
            {
                // Convert the cells into text
                string newText = new string(Cells.Where(x => x.Dots != 0).Select(x => x.Char).ToArray());
                // Update Text but not Cells
                await Update(newText, true, false);
            }
        }

        /// <summary>
        /// Updates the text box and/or Braille input cells in response to user input.
        /// </summary>
        /// <param name="newText">The latest text that was entered, or the text that should be set after the latest cell change.</param>
        /// <param name="setText">Whether to set <see cref="Text"/> equal to <paramref name="newText"/>.</param>
        /// <param name="setCells">Whether to update <see cref="Cells"/> to match the new text.</param>
        public async Task Update(string newText, bool setText, bool setCells)
        {
            // Do nothing when this flag is set; see below
            if (!_ignoreChanges)
            {
                // Just a unique number that identifies this particular Update call; see below.
                // No lock statement is used here because we know that this method only gets called on the main (UI) thread.
                int updateId = ++_updateId;

                // Start the refresh animation and remove the previous results from the list
                ListIsRefreshing = true;
                Contractions = new List<ContractionVM>(0);

                // Set Text and Cells here on the UI thread.
                // This will cause recursive Update calls, so set a flag to ignore those.
                _ignoreChanges = true;
                if (setText)
                {
                    Text = newText;
                }
                if (setCells)
                {
                    SetCells(newText);
                }
                _ignoreChanges = false;

                // Only search if we have finished loading the data file
                if (_allContractions.Count > 0)
                {
                    // Run the search on another thread. The current thread can go handle other UI business while awaiting.
                    var result = await Task.Run(() => Search(newText));

                    // If the number has not changed, set the result and end the refresh
                    if (_updateId == updateId)
                    {
                        Contractions = result;
                        ListIsRefreshing = false;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the Braille input cells to match the given text.
        /// </summary>
        /// <param name="newText">The latest text that was entered.</param>
        private void SetCells(string newText)
        {
            // If the text contains 1 to 4 Braille characters, update the input cells to match
            if (newText.Length > 0 && newText.Length <= Cells.Length && newText.All(Extensions.IsBraille))
            {
                for (int i = 0; i < Cells.Length; i++)
                {
                    Cells[i].Dots = i < newText.Length
                        // Take the lowest 8 bits because Unicode supports 8-dot cells.
                        // This app currently supports only 6-dot cells, but the 2 extra bits won't hurt.
                        ? newText[i] & 0xFF
                        : 0;
                }
            }
            // Otherwise clear all cells
            else
            {
                foreach (var cell in Cells)
                {
                    cell.Dots = 0;
                }
            }
        }

        /// <summary>
        /// Returns Braille contractions that match the given query.
        /// </summary>
        /// <param name="query">The text to search for.</param>
        private List<ContractionVM> Search(string query)
        {
            var results = query.Length == 0
                ? _allContractions.AsEnumerable()
                : _allContractions.Where(x =>
                    x.LongForm.Contains(query) ||
                    x.ShortForm.ToString().Contains(query) ||
                    x.AllBrailleShortForm.ToString().Contains(query));

            return new List<ContractionVM>(results);
        }
    }
}
