using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrailleContractions.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoPage : ContentPage
    {
        public InfoPage()
        {
            InitializeComponent();
            Title = "Info";
        }
    }
}
