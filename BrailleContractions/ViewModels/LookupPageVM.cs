using BrailleContractions.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BrailleContractions.ViewModels
{
    public class LookupPageVM : BaseVM
    {
        /// <summary>
        /// The user's search query.
        /// </summary>
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
        private string _text = string.Empty;

        /// <summary>
        /// True if the Braille input cells are visible.
        /// </summary>
        public bool CellsVisible
        {
            get => _cellsVisible;
            set => SetProperty(ref _cellsVisible, value);
        }
        private bool _cellsVisible;

        /// <summary>
        /// The RowHeight to set on the ListView.
        /// </summary>
        public int RowHeight
        {
            get => _rowHeight;
            set => SetProperty(ref _rowHeight, value);
        }
        private int _rowHeight;

        /// <summary>
        /// Height of the layout that contains the text entry.
        /// </summary>
        public int EntryContainerHeight
        {
            get => _entryContainerHeight;
            set => SetProperty(ref _entryContainerHeight, value);
        }
        private int _entryContainerHeight;

        /// <summary>
        /// Toggles the refresh animation on the ListView.
        /// </summary>
        public bool ListIsRefreshing
        {
            get => _listIsRefreshing;
            set => SetProperty(ref _listIsRefreshing, value);
        }
        private bool _listIsRefreshing = false;

        /// <summary>
        /// Contractions that are currently visible in the ListView.
        /// </summary>
        public List<ContractionVM> Contractions
        {
            get => _contractions;
            private set => SetProperty(ref _contractions, value);
        }
        private List<ContractionVM> _contractions = new List<ContractionVM>(0);
        private ContractionVM[] _sourceData = null;

        // For keeping track of searches / ListView updates
        private bool _ignoreChanges;
        private int _updateId;

        /// <summary>
        /// Reference to the global settings instance.
        /// </summary>
        public Settings Settings { get; }

        /// <summary>
        /// ViewModels of the cells with toggleable dots.
        /// </summary>
        public BrailleInputCellVM[] Cells { get; } = new BrailleInputCellVM[4];

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">App settings.</param>
        public LookupPageVM(Settings settings, DataReader dataReader)
        {
            Settings = settings;

            settings.PropertyChanged += SettingsPropertyChanged;
            AdjustForFontScale();
            
            // Construct the big Braille cells for user input
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new BrailleInputCellVM(settings);
                Cells[i].PropertyChanged += CellPropertyChanged;
            }

            // Subscribe to the data reader and fill in the ListView when it's done
            dataReader.WhenDone(data => Device.BeginInvokeOnMainThread(() =>
            {
                _sourceData = data;
                Contractions = data.ToList();
            }));
        }

        private void SettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModels.Settings.FontScale) ||
                e.PropertyName == nameof(ViewModels.Settings.FontSize))
            {
                AdjustForFontScale();
            }
        }

        private void AdjustForFontScale()
        {
            // Adjust the row height to hopefully accommodate the device's font scale setting
            RowHeight = (int)Settings.FontScale;
            EntryContainerHeight = RowHeight + 12;
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
                string newText = new string(Cells.Select(x => x.Char).ToArray()).TrimEnd('\u2800');
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
            if (!_ignoreChanges && _sourceData != null)
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

                    // HACK: Tell the view to unfocus the Entry (to hide the soft keyboard in case it appeared when we changed the text)
                    OnPropertyChanged(nameof(Entry.Unfocus));
                }
                if (setCells)
                {
                    SetCells(newText);
                }
                _ignoreChanges = false;

                // Run the search on another thread. The current thread can go handle other UI business while awaiting.
                var result = await Task.Run(() =>
                {
                    // Trim whitespace and any blank Braille characters
                    string query = newText.ToLower().Trim().Trim('\u2800');

                    var results = query.Length == 0
                        ? _sourceData.AsEnumerable()
                        : _sourceData.Where(x =>
                            (
                                (x.LongForm.Contains(query) || x.LongFormBraille.Contains(query))
                                // The first 10 entries contain parentheses in the long form and we don't want to match them
                                && query != "(" && query != ")"
                            ) ||
                            (x.Symbol != null && x.Symbol.Contains(query)) ||
                            x.ShortForm.ToString().Contains(query) ||
                            x.ShortFormBraille.ToString().Contains(query));

                    return new List<ContractionVM>(results);
                });

                // If the number has not changed, set the result and end the refresh
                if (_updateId == updateId)
                {
                    Contractions = result;
                    ListIsRefreshing = false;
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
    }
}
