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
            var lookupPage = new LookupPage(new LookupPageVM(settings));
            MainPage = new NavigationPage(lookupPage);
        }
    }
}
