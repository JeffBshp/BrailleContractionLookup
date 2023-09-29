using BrailleContractions.Services;
using UIKit;
using Xamarin.Forms;
using Size = UIKit.UIContentSizeCategory;

[assembly: Dependency(typeof(BrailleContractions.iOS.SystemService))]
namespace BrailleContractions.iOS
{
    public class SystemService : ISystemService
    {
        public string AppVersion => "1.0.0";

        public FontScale FontScale => GetFontScale(UIApplication.SharedApplication.GetPreferredContentSizeCategory());

        private static FontScale GetFontScale(Size size)
        {
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
