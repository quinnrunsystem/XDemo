using System.ComponentModel;
using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XDemo.Droid.Renderers.ExtendedElements.Separator;
using XDemo.UI.Controls.ExtendedElements;

[assembly: ExportRenderer(typeof(Separator), typeof(SeparatorRenderer))]
namespace XDemo.Droid.Renderers.ExtendedElements.Separator
{
    public class SeparatorRenderer : ViewRenderer<XDemo.UI.Controls.ExtendedElements.Separator, SeparatorDroidView>
    {

        public SeparatorRenderer(Context context) : base(context)
        {
        }
        /// <summary>
        /// Called when [element changed].
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<XDemo.UI.Controls.ExtendedElements.Separator> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
            {
                return;
            }

            if (Control == null)
            {
                SetNativeControl(new SeparatorDroidView(Context));
            }

            SetProperties();
        }


        /// <summary>
        /// Handles the <see cref="E:ElementPropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            SetProperties();
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        private void SetProperties()
        {
            Control.SpacingBefore = Element.SpacingBefore;
            Control.SpacingAfter = Element.SpacingAfter;
            Control.Thickness = Element.Thickness;
            Control.StrokeColor = Element.Color.ToAndroid();
            Control.StrokeType = Element.StrokeType;
            Control.Orientation = Element.Orientation;

            Control.Invalidate();
        }
    }
}
