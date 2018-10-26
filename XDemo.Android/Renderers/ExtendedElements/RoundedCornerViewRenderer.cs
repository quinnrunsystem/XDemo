using System;
using Android.Content;
using Android.Graphics.Drawables;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XDemo.Droid.Renderers.ExtendedElements;
using XDemo.UI.Controls.ExtendedElements;

[assembly: ExportRenderer(typeof(RoundedCornerView), typeof(RoundedCornerViewRenderer))]
namespace XDemo.Droid.Renderers.ExtendedElements
{
    public class RoundedCornerViewRenderer : ViewRenderer
    {
        public static void Init() { }
        public RoundedCornerViewRenderer(Context context) : base(context)
        {
        }
        /// <summary>
        /// ,
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null && e.NewElement != null)
            {

                RoundedCornerView rcv = ((RoundedCornerView)e.NewElement);
                var shape = new GradientDrawable();

                if (rcv.MakeCircle)
                {
                    shape.SetCornerRadius(Math.Min(Width, Height) / 2);
                }
                else
                    shape.SetCornerRadius((float)rcv.RoundedCornerRadius);
                shape.SetColor(rcv.FillColor.ToAndroid());
                shape.SetStroke((int)rcv.BorderWidth, (((RoundedCornerView)Element)).BorderColor.ToAndroid());
                this.SetBackground(shape);
            }
        }

        /// <summary>
        /// Drawables the state changed.
        /// </summary>
        protected override void DrawableStateChanged()
        {
            base.DrawableStateChanged();

            RoundedCornerView rcv = ((RoundedCornerView)Element);
            var shape = new GradientDrawable();

            if (rcv.MakeCircle)
            {
                shape.SetCornerRadius(Math.Min(Width, Height) / 2);
            }
            else
                shape.SetCornerRadius((float)rcv.RoundedCornerRadius);
            shape.SetColor(rcv.FillColor.ToAndroid());
            shape.SetStroke((int)rcv.BorderWidth, (((RoundedCornerView)Element)).BorderColor.ToAndroid());
            this.SetBackground(shape);
        }
    }
}

