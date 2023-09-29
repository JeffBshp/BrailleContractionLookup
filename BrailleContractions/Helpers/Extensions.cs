namespace BrailleContractions.Helpers
{
    internal static class Extensions
    {
        /// <summary>
        /// Returns true if the character is in the Unicode Braille range.
        /// </summary>
        /// <param name="c">The character to check.</param>
        public static bool IsBraille(this char c) => c >= 0x2800 && c <= 0x28FF;
    }
}
