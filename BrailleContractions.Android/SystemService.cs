using Android.Content.Res;
using BrailleContractions.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(BrailleContractions.Droid.SystemService))]
namespace BrailleContractions.Droid
{
    public class SystemService : ISystemService
    {
        public string AppVersion => "1.0.5";

        public FontScale FontScale => GetFontScale(Resources.System.Configuration.FontScale);

        private static FontScale GetFontScale(float scale)
        {
            // On Android the possible values are 0.85, 1.00, 1.15, and 1.30.
            // I'm using comparisons instead of exact matches to be safe.
            if (scale < 1.0)
            {
                return FontScale.Small;
            }
            else if (scale < 1.1)
            {
                return FontScale.Medium;
            }
            else if (scale < 1.2)
            {
                return FontScale.Large;
            }
            else
            {
                return FontScale.ExtraLarge;
            }
        }
    }
}
