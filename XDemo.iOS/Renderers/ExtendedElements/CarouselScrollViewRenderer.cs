using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XDemo.iOS.Renderers.ExtendedElements;
using XDemo.UI.Controls.ExtendedElements;

[assembly: ExportRenderer(typeof(HorizontalScrollView), typeof(CarouselScrollViewRenderer))]
namespace XDemo.iOS.Renderers.ExtendedElements
{
    public class CarouselScrollViewRenderer: Xamarin.Forms.Platform.iOS.ScrollViewRenderer
    {
        public CarouselScrollViewRenderer()
        {
        }   
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            Bounces = false;
            //TODO change structure
            this.DecelerationRate = 0;
            DraggingEnded += ScrollRenderer_DraggingEnded;
        }

        void ScrollRenderer_DraggingEnded(object sender, UIKit.DraggingEventArgs e)
        {
            OffsetCarouselView GetCarouselView(Element element)
            {
                if (element == null) return null;
                if (element.Parent is OffsetCarouselView carouselView) return carouselView;
                return GetCarouselView(element.Parent);
            }
            //TODO change structure
            var resultCarouselView = GetCarouselView(Element);
            resultCarouselView?.SnapHandler();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) DraggingEnded -= ScrollRenderer_DraggingEnded;
            base.Dispose(disposing);
        }
    }
}
