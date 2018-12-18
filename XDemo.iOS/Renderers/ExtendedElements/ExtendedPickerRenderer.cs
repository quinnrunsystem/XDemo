using Xamarin.Forms.Platform.iOS;
using XDemo.UI.Controls.ExtendedElements;
using XDemo.iOS.Renderers.ExtendedElements;
using Xamarin.Forms;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(ExtendedPicker), typeof(ExtendedPickerRenderer))]
namespace XDemo.iOS.Renderers.ExtendedElements
{
    public class ExtendedPickerRenderer : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                // Instantiate the native control
            }

            if (e.OldElement != null)
            {
                // Unsubscribe from event handlers and cleanup any resources
            }

            if (e.NewElement != null)
            {
                // Configure the control and subscribe to event handlers
                Control.Placeholder = (e.NewElement as ExtendedPicker)?.Placeholder;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case nameof(ExtendedPicker.Placeholder):
                    if (Control != null)
                    {
                        var picker = sender as ExtendedPicker;
                        Control.Placeholder = picker?.Placeholder;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
