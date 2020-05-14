using System;
using Xamarin.Forms;

namespace BrailleContractions.Views
{
    /// <summary>
    /// UI element for a single toggleable dot in a Braille cell.
    /// </summary>
    public class Dot : Switch
    {
        [Obsolete]
        public override SizeRequest GetSizeRequest(double widthConstraint, double heightConstraint) =>
            MakeSquare(base.GetSizeRequest(widthConstraint, heightConstraint));

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint) =>
            MakeSquare(base.OnMeasure(widthConstraint, heightConstraint));

        [Obsolete]
        protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint) =>
            MakeSquare(base.OnSizeRequest(widthConstraint, heightConstraint));

        private static SizeRequest MakeSquare(SizeRequest sizeRequest) =>
            new SizeRequest(MakeSquare(sizeRequest.Request), MakeSquare(sizeRequest.Minimum));

        private static Size MakeSquare(Size size)
        {
            var min = Math.Min(size.Width, size.Height);
            return new Size(min, min);
        }
    }
}
