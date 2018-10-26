using System;
using System.Diagnostics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XDemo.iOS.Renderers.ExtendedElements;
using XDemo.UI.Controls.ExtendedElements;

[assembly: ExportRenderer(typeof(RoundedCornerView), typeof(RoundedCornerViewRenderer))]

namespace XDemo.iOS.Renderers.ExtendedElements
{
    public class RoundedCornerViewRenderer : ViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (Element == null) return;
            Element.PropertyChanged += (sender, e1) =>
            {
                try
                {
                    if (NativeView != null)
                    {
                        NativeView.SetNeedsDisplay();
                        NativeView.SetNeedsLayout();
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Handled Exception in RoundedCornerViewDemoRenderer. Just warning : " + exp.Message);
                }
            };
        }
        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);
            LayoutIfNeeded();
            var rcv = (RoundedCornerView)Element;
            //rcv.Padding = new Thickness(0, 0, 0, 0);
            ClipsToBounds = true;
            Layer.BackgroundColor = rcv.FillColor.ToCGColor();
            Layer.MasksToBounds = true;
            Layer.CornerRadius = (nfloat)rcv.RoundedCornerRadius;
            if (rcv.MakeCircle)
            {
                Layer.CornerRadius = (int)(Math.Min(Element.Width, Element.Height) / 2);
            }
            Layer.BorderWidth = 0;
            if (rcv.BorderWidth > 0 && rcv.BorderColor.A > 0.0)
            {
                Layer.BorderWidth = rcv.BorderWidth;
                Layer.BorderColor = new UIColor(
                    (nfloat)rcv.BorderColor.R,
                    (nfloat)rcv.BorderColor.G,
                    (nfloat)rcv.BorderColor.B,
                    (nfloat)rcv.BorderColor.A).CGColor;
            }
        }
    }
}