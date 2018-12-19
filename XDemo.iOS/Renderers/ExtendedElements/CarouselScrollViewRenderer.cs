using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XDemo.iOS.Renderers.ExtendedElements;
using XDemo.UI.Controls.GroupedElements.CarouselScrollViews;

[assembly: ExportRenderer(typeof(CarouselScrollView), typeof(CarouselScrollViewRenderer))]
namespace XDemo.iOS.Renderers.ExtendedElements
{
    public class CarouselScrollViewRenderer : ScrollViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                // Configure the control and subscribe to event handlers
                Bounces = false;
                this.DecelerationRate = 0;
                DraggingEnded += ScrollRenderer_DraggingEnded;
            }
            if (e.OldElement != null)
            {
                // Unsubscribe from event handlers and cleanup any resources
                DraggingEnded -= ScrollRenderer_DraggingEnded;
            }
        }

        void ScrollRenderer_DraggingEnded(object sender, UIKit.DraggingEventArgs e)
        {
            CarouselScrollViewLayout GetCarouselView(Element element)
            {
                if (element == null) return null;
                if (element.Parent is CarouselScrollViewLayout carouselView) return carouselView;
                return GetCarouselView(element.Parent);
            }
         
            var resultCarouselView = GetCarouselView(Element);
            resultCarouselView?.SnapHandler();
        }
    }
}
