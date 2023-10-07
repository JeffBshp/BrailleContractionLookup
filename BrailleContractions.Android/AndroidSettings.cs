using Android.Content.Res;
using BrailleContractions.ViewModels;

namespace BrailleContractions.Droid
{
    internal class AndroidSettings : Settings
    {
        public override string AppVersion => "1.0.5";

        public override string BackspaceIcon => "outline_keyboard_backspace_24";

        public override string ClearIcon => "outline_clear_24";

        public override string InfoIcon => "outline_info_24";

        internal AndroidSettings()
        {
            FontScaleChanged(Resources.System.Configuration);
        }

        internal void FontScaleChanged(Configuration newConfig)
        {
            // FontScale may have changed. Convert it to the enum and get the associated absolute size.
            // There is also a density setting (called "Display size" under accessibility settings),
            // but I'm ignoring that because it varies wildly by device.
            FontScale = GetFontScale(newConfig.FontScale, out double pointSize);
            FontSize = pointSize;
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
