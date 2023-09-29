using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Xamarin.Forms.Platform.Android;

namespace BrailleContractions.Droid
{
    [Activity(Theme = "@style/MainTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : FormsAppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();
            new Task(StartMainActivity).Start();
        }

        private void StartMainActivity()
        {
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            Finish(); // End the splash activity
        }
    }
}
