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
        public static void Init() { }
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (this.Element == null) return;
            this.Element.PropertyChanged += (sender, e1) =>
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
                    Debug.WriteLine("Handled Exception in RoundedCornerViewDemoRenderer. Just warngin : " + exp.Message);
                }
            };
        }
        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);
            this.LayoutIfNeeded();
            RoundedCornerView rcv = (RoundedCornerView)Element;
            //rcv.Padding = new Thickness(0, 0, 0, 0);
            this.ClipsToBounds = true;
            this.Layer.BackgroundColor = rcv.FillColor.ToCGColor();
            this.Layer.MasksToBounds = true;
            this.Layer.CornerRadius = (nfloat)rcv.RoundedCornerRadius;
            if (rcv.MakeCircle)
            {
                this.Layer.CornerRadius = (int)(Math.Min(Element.Width, Element.Height) / 2);
            }
            this.Layer.BorderWidth = 0;
            if (rcv.BorderWidth > 0 && rcv.BorderColor.A > 0.0)
            {
                this.Layer.BorderWidth = rcv.BorderWidth;
                this.Layer.BorderColor = new UIColor(
                    (nfloat)rcv.BorderColor.R,
                    (nfloat)rcv.BorderColor.G,
                    (nfloat)rcv.BorderColor.B,
                    (nfloat)rcv.BorderColor.A).CGColor;
            }
        }
    }
}