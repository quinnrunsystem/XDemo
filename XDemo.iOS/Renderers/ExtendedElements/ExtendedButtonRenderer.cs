using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XDemo.UI.Controls.ExtendedElements;
using XDemo.iOS.Renderers.ExtendedElements;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using System.ComponentModel;
using UIKit;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]
namespace XDemo.iOS.Renderers.ExtendedElements
{
    public class ExtendedButtonRenderer : ButtonRenderer
    {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case nameof(ExtendedButton.Width):
                case nameof(ExtendedButton.Height):
                case nameof(ExtendedButton.CornerRadius):
                case nameof(ExtendedButton.GradientColors):
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
            if (gradientColors == null || !gradientColors.Any())
                return;

            /* ==================================================================================================
             * create the gradient
             * ================================================================================================*/
            var gradient = new CAGradientLayer();
            var tempRect = new CGRect(0, 0, extendedButton.Width, extendedButton.Height);
            gradient.Frame = tempRect;

            /* ==================================================================================================
             * Need to convert the colors to CGColor objects
             * ================================================================================================*/
            var cgColors = gradientColors.Select(arg => arg.ToCGColor()).ToArray();
            gradient.Colors = cgColors;

            /* ==================================================================================================
             * set the flow
             * ================================================================================================*/
            switch (extendedButton.GradientFlow)
            {
                case ExtendedButton.Flows.Horizontal:
                    gradient.StartPoint = new CGPoint(0, 0.5);
                    gradient.EndPoint = new CGPoint(1, 0.5);
                    break;
                case ExtendedButton.Flows.Vertical:
                    gradient.StartPoint = new CGPoint(0.5, 0);
                    gradient.EndPoint = new CGPoint(0.5, 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"The {nameof(ExtendedButton.GradientFlow)}: {extendedButton.GradientFlow} is not supported yet!");
            }

            /* ==================================================================================================
             * update corners
             * ================================================================================================*/
            var rec = new CGRect(0, 0, extendedButton.Width, extendedButton.Height);
            var rounded = UIRectCorner.TopLeft | UIRectCorner.TopRight | UIRectCorner.BottomLeft | UIRectCorner.BottomRight;
            var path = UIBezierPath.FromRoundedRect(rec, rounded, new CGSize(extendedButton.CornerRadius, extendedButton.CornerRadius));
            gradient.Mask = new CAShapeLayer
            {
                Path = path.CGPath
            };

            /* ==================================================================================================
             * add or replace the old layer if added
             * ================================================================================================*/
            var lastLayer = Control?.Layer.Sublayers.FirstOrDefault();
            if (lastLayer is CAGradientLayer)
                Control.Layer.ReplaceSublayer(lastLayer, gradient);
            else
                Control?.Layer.InsertSublayerBelow(gradient, lastLayer);
        }
    }
}
