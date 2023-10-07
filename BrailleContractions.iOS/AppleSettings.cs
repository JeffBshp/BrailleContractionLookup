using BrailleContractions.ViewModels;
using UIKit;
using Size = UIKit.UIContentSizeCategory;

namespace BrailleContractions.iOS
{
    internal class AppleSettings : Settings
    {
        public override string AppVersion => "1.0.0";

        public override string BackspaceIcon => "outline_keyboard_backspace_black_48pt";

        public override string ClearIcon => "outline_clear_black_48pt";

        public override string InfoIcon => "outline_info_black_24pt";

        internal AppleSettings()
        {
            HandleContentSizeCategoryChanged(null, null);
        }

        internal void HandleContentSizeCategoryChanged(object sender, UIContentSizeCategoryChangedEventArgs args)
        {
            FontScale = GetFontScale();
            FontSize = UIFont.PreferredBody.PointSize;
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
