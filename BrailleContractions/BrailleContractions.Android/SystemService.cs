using Android.Content.Res;
using BrailleContractions.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(BrailleContractions.Droid.SystemService))]
namespace BrailleContractions.Droid
{
    public class SystemService : ISystemService
    {
        public double FontScale => Resources.System.Configuration.FontScale;
    }
}
