using BrailleContractions.Helpers;
using BrailleContractions.ViewModels;
using BrailleContractions.Views;
using Xamarin.Forms;

// Register the custom font for use throughout the app.
// No need to specify the folder it's in.
[assembly: ExportFont("JBraille.ttf", Alias = "JBraille")]
namespace BrailleContractions
{
    public partial class App : Application
    {
        public App(Settings settings)
        {
            InitializeComponent();
            BindingContext = settings;
            var dataReader = new DataReader(settings);
            var lookupPage = new LookupPage(new LookupPageVM(settings, dataReader));
            MainPage = new NavigationPage(lookupPage);
        }
    }
}
