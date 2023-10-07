using BrailleContractions.Helpers;
using BrailleContractions.iOS;
using BrailleContractions.Views;
using CoreAnimation;
using CoreGraphics;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

// Indicate that the DotRenderer class should be used to render the Dot view.
[assembly: ExportRenderer(typeof(Dot), typeof(DotRenderer))]
namespace BrailleContractions.iOS
{
    internal class DotRenderer : ButtonRenderer
    {
        private static readonly CGColorSpace _colorSpace = CGColorSpace.CreateGenericGray();
        private static readonly CGColor _color = CGColor.CreateGenericGrayGamma2_2(0, 1);

        public override void DrawLayer(CALayer layer, CGContext context)
        {
            // Construct a smaller box with room for 1/2 stroke width on each side
            double outerDiameter = Math.Min(Bounds.Width, Bounds.Height);
            double strokeWidth = Math.Round(outerDiameter / Constants.DotDiameterToStrokeRatio);
            double diameter = outerDiameter - strokeWidth;
            var bounds = new CGRect(strokeWidth / 2d, strokeWidth / 2d, diameter, diameter);

            // Draw a circle
            context.SetStrokeColorSpace(_colorSpace);
            context.SetStrokeColor(_color);
            context.SetLineWidth(new nfloat(strokeWidth));
            context.StrokeEllipseInRect(bounds);

            // Fill it in
            if (((Dot)Element).IsFilled)
            {
                context.SetFillColorSpace(_colorSpace);
                context.SetFillColor(_color);
                context.FillEllipseInRect(bounds);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Require a redraw when the dot changes state
            if (e.PropertyName == nameof(Dot.IsFilled))
            {
                SetNeedsDisplay();
            }

            base.OnElementPropertyChanged(sender, e);
        }
    }
}
