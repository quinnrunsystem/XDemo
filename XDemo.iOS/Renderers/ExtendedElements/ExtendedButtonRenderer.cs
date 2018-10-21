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
                var oldLayer = Control?.Layer.Sublayers.FirstOrDefault();
                if (oldLayer is CAGradientLayer)
                    oldLayer.RemoveFromSuperLayer();
                return;
            }

            /* ==================================================================================================
             * create the gradient
             * ================================================================================================*/
            var gradient = new CAGradientLayer
            {
                /* ==================================================================================================
                 * the frame's size must be the same with owner view
                 * ================================================================================================*/
                Frame = new CGRect(0, 0, extendedButton.Width, extendedButton.Height),
                /* ==================================================================================================
                 * convert the colors to CGColor objects
                 * ================================================================================================*/
                Colors = gradientColors.Select(arg => arg.ToCGColor()).ToArray()
            };

            /* ==================================================================================================
             * set the flow
             * ================================================================================================*/
            switch (extendedButton.GradientFlow)
            {
                case ExtendedButton.Flows.LeftToRight:
                    gradient.StartPoint = new CGPoint(0, 0.5);
                    gradient.EndPoint = new CGPoint(1, 0.5);
                    break;
                case ExtendedButton.Flows.TopDown:
                    gradient.StartPoint = new CGPoint(0.5, 0);
                    gradient.EndPoint = new CGPoint(0.5, 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"The {nameof(ExtendedButton.GradientFlow)}: {extendedButton?.GradientFlow.ToString() ?? "-"} is not supported yet!");
            }

            /* ==================================================================================================
             * update corners
             * ================================================================================================*/
            var rec = new CGRect(0, 0, extendedButton.Width, extendedButton.Height);
            var allRounded = UIRectCorner.TopLeft | UIRectCorner.TopRight | UIRectCorner.BottomLeft | UIRectCorner.BottomRight;
            var path = UIBezierPath.FromRoundedRect(rec, allRounded, new CGSize(extendedButton.CornerRadius, extendedButton.CornerRadius));
            gradient.Mask = new CAShapeLayer
            {
                Path = path.CGPath
            };

            /* ==================================================================================================
             * add or replace the old layer if added
             * ================================================================================================*/
            var existedLayer = Control?.Layer.Sublayers.FirstOrDefault();
            if (existedLayer is CAGradientLayer)
                Control?.Layer.ReplaceSublayer(existedLayer, gradient);
            else
                Control?.Layer.InsertSublayerBelow(gradient, existedLayer);
        }
    }
}
