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
            var dataReader = new DataReader(settings);
            var lookupPage = new LookupPage(new LookupPageVM(settings, dataReader));
            MainPage = new NavigationPage(lookupPage);
        }
    }
}
