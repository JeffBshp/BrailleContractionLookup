using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using BrailleContractions.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Settings = BrailleContractions.ViewModels.Settings;

namespace BrailleContractions.Droid
{
    [Activity(
        Label = "Braille Contractions",
        Icon = "@drawable/icon",
        Theme = "@style/MainTheme",
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges =
            ConfigChanges.Density |
            ConfigChanges.FontScale |
            ConfigChanges.Orientation |
            ConfigChanges.ScreenLayout |
            ConfigChanges.ScreenSize)]
    public class MainActivity : FormsAppCompatActivity
    {
        private Settings _settings;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            var fontScale = GetFontScale(Resources.System.Configuration.FontScale, out double pointSize);
            _settings = new Settings("1.0.5", fontScale, pointSize);
            LoadApplication(new App(_settings));
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            // FontScale may have changed. Convert it to the enum and get the associated absolute size.
            // There is also a density setting (called "Display size" under accessibility settings),
            // but I'm ignoring that because it varies wildly by device.
            _settings.FontScale = GetFontScale(newConfig.FontScale, out double pointSize);
            _settings.FontSize = pointSize;

            base.OnConfigurationChanged(newConfig);
        }

        private static FontScale GetFontScale(float scale, out double pointSize)
        {
            // On Android the possible scale values are 0.85, 1.00, 1.15, and 1.30.
            // I'm using comparisons instead of exact matches to be safe.
            // For pointSize, we should be able to call Device.GetNamedSize(NamedSize.Large, typeof(Label))
            // but that doesn't seem to return the updated value, even when Fontscale has changed,
            // until the whole app restarts. So hardcoded numbers are used instead.

            if (scale < 1.0)
            {
                pointSize = 20;
                return FontScale.Small;
            }
            else if (scale < 1.1)
            {
                pointSize = 23;
                return FontScale.Medium;
            }
            else if (scale < 1.2)
            {
                pointSize = 26;
                return FontScale.Large;
            }
            else
            {
                pointSize = 29;
                return FontScale.ExtraLarge;
            }
        }
    }
}
