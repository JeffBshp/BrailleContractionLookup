using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

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
        private AndroidSettings _settings;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Forms.Init(this, bundle);

            _settings = new AndroidSettings();
            LoadApplication(new App(_settings));
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            _settings.FontScaleChanged(newConfig);
            base.OnConfigurationChanged(newConfig);
        }
    }
}
