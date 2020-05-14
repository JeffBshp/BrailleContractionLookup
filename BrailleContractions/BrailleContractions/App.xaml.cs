using BrailleContractions.Helpers;
using BrailleContractions.ViewModels;
using BrailleContractions.Views;
using Xamarin.Forms;

namespace BrailleContractions
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var settings = new Settings();
            var uebContractions = DataReader.ReadUebContractions(settings);
            var lookupPage = new LookupPage(new LookupPageVM(settings, uebContractions));
            MainPage = new NavigationPage(lookupPage);
        }
    }
}
