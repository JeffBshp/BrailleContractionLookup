using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrailleContractions.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoPage : ContentPage
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="appVersion">App version to be shown to the user.</param>
        public InfoPage(string appVersion)
        {
            InitializeComponent();
            Title = "Info";
            AppVersionText.Text = $"Version {appVersion}";
        }
    }
}
