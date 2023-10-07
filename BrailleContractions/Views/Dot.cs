using System;
using Xamarin.Forms;

namespace BrailleContractions.Views
{
    /// <summary>
    /// UI element for a single toggleable dot in a Braille cell.
    /// This is a <see cref="Button"/> instead of a <see cref="Switch"/>
    /// because of issues with the custom renderer on iOS.
    /// </summary>
    public class Dot : Button
    {
        /// <summary>
        /// Copied from <see cref="Switch.IsToggledProperty"/>.
        /// </summary>
        public static readonly BindableProperty IsFilledProperty = BindableProperty.Create(nameof(IsFilled), typeof(bool), typeof(Dot), false, propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((Dot)bindable).Toggled?.Invoke(bindable, new ToggledEventArgs((bool)newValue));
            ((Dot)bindable).ChangeVisualState();
        }, defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// True if the dot is filled.
        /// </summary>
        public bool IsFilled
        {
            get => (bool)GetValue(IsFilledProperty);
            set => SetValue(IsFilledProperty, value);
        }

        /// <summary>
        /// Triggered when the dot is toggled.
        /// </summary>
        public event EventHandler<ToggledEventArgs> Toggled;

        public Dot()
        {
            Clicked += (s, e) => IsFilled = !IsFilled;
        }

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
