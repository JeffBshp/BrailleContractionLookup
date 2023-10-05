using BrailleContractions.ViewModels;
using Foundation;
using UIKit;
using Size = UIKit.UIContentSizeCategory;

namespace BrailleContractions.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private NSObject _notificationToken;
        private Settings _settings;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Forms.Forms.Init();
            _settings = new Settings("1.0.0", GetFontScale(), UIFont.PreferredBody.PointSize);
            _notificationToken = UIApplication.Notifications.ObserveContentSizeCategoryChanged(HandleContentSizeCategoryChanged);
            LoadApplication(new App(_settings));

            return base.FinishedLaunching(app, options);
        }

        public override void WillTerminate(UIApplication app)
        {
            _notificationToken.Dispose();
            base.WillTerminate(app);
        }

        private void HandleContentSizeCategoryChanged(object sender, UIContentSizeCategoryChangedEventArgs args)
        {
            _settings.FontScale = GetFontScale();
            _settings.FontSize = UIFont.PreferredBody.PointSize;
        }

        private static FontScale GetFontScale()
        {
            var size = UIApplication.SharedApplication.GetPreferredContentSizeCategory();

            switch (size)
            {
                case Size.Unspecified:
                    goto case Size.Large;
                case Size.ExtraSmall:
                case Size.Small:
                case Size.Medium:
                    return FontScale.Small;
                case Size.Large:
                case Size.ExtraLarge:
                case Size.ExtraExtraLarge:
                    return FontScale.Medium;
                case Size.ExtraExtraExtraLarge:
                case Size.AccessibilityMedium:
                case Size.AccessibilityLarge:
                    return FontScale.Large;
                case Size.AccessibilityExtraLarge:
                case Size.AccessibilityExtraExtraLarge:
                case Size.AccessibilityExtraExtraExtraLarge:
                    return FontScale.ExtraLarge;
                default:
                    goto case Size.Large;
            }
        }
    }
}
