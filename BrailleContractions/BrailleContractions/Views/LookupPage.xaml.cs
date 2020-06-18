using BrailleContractions.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrailleContractions.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LookupPage : ContentPage
    {
        public LookupPage(LookupPageVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            Title = "Braille Contractions";
            ToolbarItems.Add(new ToolbarItem("Info", "outline_info_black_48",
                () => Navigation.PushAsync(new InfoPage())));
            SizeChanged += PageSizeChanged;
        }

        /// <summary>
        /// Updates the size of the dots to fit on the page.
        /// This is the only way I have found to satisfy the following constraints:
        /// - All dots are the same size
        /// - Dots scale up to a maximum size, and then the four cells are centered horizontally
        /// - Dots scale down to a minimum size, and then the four cells are hidden
        /// - Enough vertical space is reserved for a minimum number of rows in the ListView
        /// </summary>
        private void PageSizeChanged(object sender, EventArgs e)
        {
            // Width reserved for margin/spacing between dots
            const int usedWidth = 66;
            // Height reserved for margin/spacing and the text box
            const int usedHeight = 84;
            const int minRowsVisible = 3;
            const int minDotSize = 20;
            const int maxDotSize = 36;

            var viewModel = (LookupPageVM)BindingContext;
            int maxDotWidth = ((int)Width - usedWidth) / 8;
            int maxDotHeight = ((int)Height - usedHeight - (int)(viewModel.RowHeight * minRowsVisible)) / 3;
            int dotSize = Math.Min(maxDotSize, Math.Min(maxDotWidth, maxDotHeight));

            viewModel.CellsVisible = dotSize >= minDotSize;
            viewModel.Settings.DotSize = dotSize;
        }

        private async void TextChanged(object sender, TextChangedEventArgs e)
        {
            var viewModel = (LookupPageVM)BindingContext;
            // Update Cells but not Text
            await viewModel.Update(e.NewTextValue, false, true);
        }

        private async void ClickedBackspace(object sender, EventArgs e)
        {
            var viewModel = (LookupPageVM)BindingContext;
            string newText = viewModel.Text;

            if (newText.Length > 0)
            {
                newText = newText.Substring(0, newText.Length - 1);
            }

            // Update both Text and Cells
            await viewModel.Update(newText, true, true);
        }

        private async void ClickedClear(object sender, EventArgs e)
        {
            var viewModel = (LookupPageVM)BindingContext;
            // Update both Text and Cells
            await viewModel.Update(string.Empty, true, true);
        }

        private void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var viewModel = (LookupPageVM)BindingContext;
            // Toggle this setting when an item is tapped
            viewModel.Settings.DisplayShortFormInBraille = !viewModel.Settings.DisplayShortFormInBraille;
        }
    }
}
