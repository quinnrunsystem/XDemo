using System.ComponentModel;
using System.Drawing;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XDemo.iOS.Renderers.ExtendedElements;
using XDemo.UI.Controls.ExtendedElements;

[assembly: ExportRenderer(typeof(ExtendedFrame), typeof(ExtendedFrameRenderer))]
namespace XDemo.iOS.Renderers.ExtendedElements
{
    public class ExtendedFrameRenderer : FrameRenderer
    {

        public static void Init() { }
        private ExtendedFrame customFrame;

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                customFrame = e.NewElement as ExtendedFrame;
                SetupLayer();

            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
                e.PropertyName == Xamarin.Forms.Frame.OutlineColorProperty.PropertyName ||
                e.PropertyName == Xamarin.Forms.Frame.HasShadowProperty.PropertyName ||
                e.PropertyName == Xamarin.Forms.Frame.CornerRadiusProperty.PropertyName)
            {
                SetupLayer();
            }
        }

        void SetupLayer()
        {
            float cornerRadius = customFrame.CornerRadius;

            if (cornerRadius == -1f)
                cornerRadius = 5f; // default corner radius

            Layer.CornerRadius = cornerRadius;
            Layer.BackgroundColor = customFrame.BackgroundColor.ToCGColor();

            if (customFrame.HasShadow)
            {
                Layer.ShadowRadius = 2;
                Layer.ShadowColor = UIColor.Black.CGColor;
                Layer.ShadowOpacity = 0.3f;
                Layer.ShadowOffset = new SizeF();
            }
            else
                Layer.ShadowOpacity = 0;

            //if (customFrame.OutlineColor == Color.Default)
            //    Layer.BorderColor = UIColor.Clear.CGColor;
            //else
            //{
            Layer.BorderColor = customFrame.OutlineColor.ToCGColor();
            Layer.BorderWidth = customFrame.BorderThickness;
            // }

            Layer.RasterizationScale = UIScreen.MainScreen.Scale;
            Layer.ShouldRasterize = true;
        }
    }
}