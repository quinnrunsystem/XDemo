using System;
using Android.Content;
using Xamarin.Forms.Platform.Android;
using XDemo.UI.Controls.ExtendedElements;
using XDemo.Droid.Renderers.ExtendedElements;
using Xamarin.Forms;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(ExtendedPicker), typeof(ExtendedPickerRenderer))]
namespace XDemo.Droid.Renderers.ExtendedElements
{
    public class ExtendedPickerRenderer : PickerRenderer
    {
        public ExtendedPickerRenderer(Context context) : base(context)
        {
        }

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
                var picker = e.NewElement as ExtendedPicker;
                Control.Hint = picker?.Placeholder;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case nameof(ExtendedPicker.Placeholder):
                    var picker = sender as ExtendedPicker;
                    Control.Hint = picker?.Placeholder;
                    break;
                default:
                    break;
            }
        }
    }
}
