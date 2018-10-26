using System.ComponentModel;
using Android.Content;
using Android.Graphics.Drawables;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XDemo.Droid.Renderers.ExtendedElements;
using XDemo.UI.Controls.ExtendedElements;

[assembly: ExportRenderer(typeof(ExtendedFrame), typeof(ExtendedFrameRenderer))]
namespace XDemo.Droid.Renderers.ExtendedElements
{
    public class ExtendedFrameRenderer : FrameRenderer
    {
        public static void Init() { }
        GradientDrawable _gi;

        public ExtendedFrameRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement is ExtendedFrame origFrame)
            {
                var gi = new GradientDrawable();

                _gi = gi;

                gi.SetStroke(origFrame.BorderThickness, origFrame.BorderColor.ToAndroid());
                gi.SetColor(origFrame.BackgroundColor.ToAndroid());
                gi.SetCornerRadius(origFrame.CornerRadius);
//                SetBackgroundDrawable(gi);
                this.SetBackground(gi);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ChildCount > 0 && _gi != null)
            {
//                SetBackgroundDrawable(_gi);
                this.SetBackground(_gi);
            }

            base.OnElementPropertyChanged(sender, e);
        }
    }
}