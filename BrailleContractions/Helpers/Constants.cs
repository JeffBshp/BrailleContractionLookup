namespace BrailleContractions.Helpers
{
    public static class Constants
    {
        /// <summary>
        /// Stroke width of the Braille dots will be set to satisfy this ratio.
        /// Rather than hardcoding the number of pixels,
        /// this makes the stroke width consistent regardless of pixel density.
        /// </summary>
        public const double DotDiameterToStrokeRatio = 12d;

        public const double DefaultMargin = 5;
        public const double DotSpacing = 2;
        public const double InputCellSpacing = 16;

        /// <summary>
        /// Width reserved for margins and dot spacing.
        /// </summary>
        public const int ReservedWidth = (int)((DefaultMargin * 2) + (InputCellSpacing * 3) + (DotSpacing * 4));

        /// <summary>
        /// Height reserved for top/bottom margins and between Grid rows.
        /// </summary>
        public const int ReservedHeight = (int)(DefaultMargin * 4);
    }
}
