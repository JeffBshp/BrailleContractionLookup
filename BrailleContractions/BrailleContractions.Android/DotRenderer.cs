using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Views;
using BrailleContractions.Droid;
using BrailleContractions.Views;
using Xamarin.Forms.Platform.Android;
using Switch = Xamarin.Forms.Switch;

// Indicate that the DotRenderer class should be used to render the Dot view.
[assembly: Xamarin.Forms.ExportRenderer(typeof(Dot), typeof(DotRenderer))]
namespace BrailleContractions.Droid
{
    /// <summary>
    /// Renders a switch as a circle that is either empty or filled, depending on the state of the switch.
    /// </summary>
    public class DotRenderer : SwitchRenderer
    {
        /// <summary>
        /// Stroke width will be set to satisfy this ratio.
        /// Rather than hardcoding the number of physical pixels,
        /// this makes the stroke width consistent regardless of pixel density.
        /// </summary>
        private const double DiameterToStrokeRatio = 12d;

        public DotRenderer(Context context) : base(context) { }

        protected override bool DrawChild(Canvas canvas, View child, long drawingTime)
        {
            // Define a circle that fills the canvas
            var path = new Path();
            var bounds = canvas.ClipBounds;
            int minDimension = Math.Min(bounds.Width(), bounds.Height());
            // This may round to zero but Android should always draw at least one pixel
            float strokeWidth = (float)Math.Round(minDimension / DiameterToStrokeRatio);
            float radius = (minDimension - strokeWidth) / 2f;
            path.AddCircle(bounds.ExactCenterX(), bounds.ExactCenterY(), radius, Path.Direction.Cw);

            var paint = new Paint(PaintFlags.AntiAlias) { Color = Color.Black, StrokeWidth = strokeWidth };
            // Fill inside the circle if the switch is toggled on
            paint.SetStyle(Element.IsToggled ? Paint.Style.FillAndStroke : Paint.Style.Stroke);
            // Draw the circle
            canvas.DrawPath(path, paint);

            return true;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Require a redraw when the switch changes state
            if (e.PropertyName == nameof(Switch.IsToggled))
            {
                Invalidate();
            }

            base.OnElementPropertyChanged(sender, e);
        }
    }
}
