using System;
using Xamarin.Forms;

namespace XDemo.UI.Controls.ExtendedElements
{
    public class RoundedCornerView : View //Grid
    {
        public static readonly BindableProperty FillColorProperty = BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(RoundedCornerView), Color.White, BindingMode.TwoWay);
        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }

        public static readonly BindableProperty RoundedCornerRadiusProperty = BindableProperty.Create(nameof(RoundedCornerRadius), typeof(double), typeof(RoundedCornerView), 3, BindingMode.TwoWay);
        public double RoundedCornerRadius
        {
            get => (double)GetValue(RoundedCornerRadiusProperty);
            set => SetValue(RoundedCornerRadiusProperty, value);
        }

        public static readonly BindableProperty MakeCircleProperty = BindableProperty.Create(nameof(MakeCircle), typeof(bool), typeof(RoundedCornerView), default(bool)); // BindableProperty.Create<RoundedCornerView, Boolean>(w => w.MakeCircle, false);
        public Boolean MakeCircle
        {
            get => (Boolean)GetValue(MakeCircleProperty);
            set => SetValue(MakeCircleProperty, value);
        }

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(RoundedCornerView), Color.Transparent);
        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(nameof(BorderWidth), typeof(int), typeof(RoundedCornerView), 1);
        public int BorderWidth
        {
            get => (int)GetValue(BorderWidthProperty);
            set => SetValue(BorderWidthProperty, value);
        }
    }
}