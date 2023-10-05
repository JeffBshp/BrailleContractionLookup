namespace BrailleContractions.Helpers
{
    public static class Constants
    {
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
