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

            var origFrame = e.NewElement as ExtendedFrame;

            if (origFrame != null)
            {
                GradientDrawable gi = new GradientDrawable();

                _gi = gi;

                gi.SetStroke(origFrame.BorderThickness, origFrame.OutlineColor.ToAndroid());
                gi.SetColor(origFrame.BackgroundColor.ToAndroid());
                gi.SetCornerRadius(origFrame.CornerRadius);
#pragma warning disable CS0618 // Type or member is obsolete
                SetBackgroundDrawable(gi);
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ChildCount > 0 && _gi != null)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                SetBackgroundDrawable(_gi);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            base.OnElementPropertyChanged(sender, e);
        }
    }
}