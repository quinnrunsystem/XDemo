using Android.Content;
using Xamarin.Forms.Platform.Android.AppCompat;
using Xamarin.Forms;
using XDemo.UI.Controls.ExtendedElements;
using XDemo.Droid.Renderers.ExtendedElements;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using System;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]
namespace XDemo.Droid.Renderers.ExtendedElements
{
    public class ExtendedButtonRenderer : Xamarin.Forms.Platform.Android.AppCompat.ButtonRenderer
    {
        public ExtendedButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case nameof(ExtendedButton.Width):
                case nameof(ExtendedButton.Height):
                case nameof(ExtendedButton.CornerRadius):
                case nameof(ExtendedButton.GradientColors):
                case nameof(ExtendedButton.GradientFlow):
                    UpdateGradientColors();
                    break;
                default:
                    break;
            }
        }

        private void UpdateGradientColors()
        {
            var extendedButton = Element as ExtendedButton;
            var gradientColors = extendedButton?.GradientColors;
            if (gradientColors == null || !gradientColors.Any() || extendedButton.Width <= 0 || extendedButton.Height <= 0)
            {
                return;
            }
            /* ==================================================================================================
             * convert the colors to Android Color objects
             * ================================================================================================*/
            var androidColors = gradientColors.Select(arg => (int)arg.ToAndroid()).ToArray();
           
            /* ==================================================================================================
             * create the gradient
             * ================================================================================================*/
            var gradient = new GradientDrawable(GradientDrawable.Orientation.LeftRight, androidColors);
            /* ==================================================================================================
             * set the flow
             * ================================================================================================*/
            switch (extendedButton.GradientFlow)
            {
                case ExtendedButton.Flows.LeftToRight:
                    gradient.SetOrientation(GradientDrawable.Orientation.LeftRight);
                    break;
                case ExtendedButton.Flows.TopDown:
                    gradient.SetOrientation(GradientDrawable.Orientation.TopBottom);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"The {nameof(ExtendedButton.GradientFlow)}: {extendedButton?.GradientFlow.ToString() ?? "-"} is not supported yet!");
            }
            /* ==================================================================================================
             * update conrner radius
             * ================================================================================================*/
            var cornerRadius = extendedButton.CornerRadius;
            gradient.SetCornerRadii(new float[] { cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius });
            /* ==================================================================================================
             * finish up
             * ================================================================================================*/
            Control?.SetBackground(gradient);
        }
    }
}
