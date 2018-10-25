// https://baglabs.com/2017/07/14/creating-gradients-xamarin-forms/
using Android.Content;
using Xamarin.Forms;
using XDemo.UI.Controls.ExtendedElements;
using XDemo.Droid.Renderers.ExtendedElements;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using System;
using Android.Content.Res;
using Android.Support.V7.Widget;
using System.Threading.Tasks;
using XDemo.Core.Infrastructure.Logging;
using SQLitePCL;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]
namespace XDemo.Droid.Renderers.ExtendedElements
{
    public class ExtendedButtonRenderer : Xamarin.Forms.Platform.Android.AppCompat.ButtonRenderer
    {

        private readonly object _lockObj = new object();
        private bool _isEventSet = false;
        private GradientDrawable _gradient;

        public ExtendedButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case nameof(ExtendedButton.Width):
                case nameof(ExtendedButton.Height):
                case nameof(ExtendedButton.CornerRadius):
                case nameof(ExtendedButton.GradientColors):
                case nameof(ExtendedButton.GradientFlow):
                    UpdateGradientColors();
                    break;
                default:
                    break;
            }
        }

        private void UpdateGradientColors()
        {
            var extendedButton = Element as ExtendedButton;
            var gradientColors = extendedButton?.GradientColors;
            if (gradientColors == null || !gradientColors.Any() || extendedButton.Width <= 0 || extendedButton.Height <= 0)
            {
                return;
            }

            /* ==================================================================================================
             * convert the colors to Android Color objects
             * ================================================================================================*/
            var androidColors = gradientColors.Select(arg => (int)arg.ToAndroid()).ToArray();

            /* ==================================================================================================
             * create the gradient
             * ================================================================================================*/
            _gradient = _gradient ?? new GradientDrawable(GradientDrawable.Orientation.LeftRight, androidColors);
            /* ==================================================================================================
             * set the flow
             * ================================================================================================*/
            switch (extendedButton.GradientFlow)
            {
                case ExtendedButton.Flows.LeftToRight:
                    _gradient.SetOrientation(GradientDrawable.Orientation.LeftRight);
                    break;
                case ExtendedButton.Flows.TopDown:
                    _gradient.SetOrientation(GradientDrawable.Orientation.TopBottom);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"The {nameof(ExtendedButton.GradientFlow)}: {extendedButton?.GradientFlow.ToString() ?? "-"} is not supported yet!");
            }

            /* ==================================================================================================
             * update conrner radius
             * ================================================================================================*/
            var cornerRadius = extendedButton.CornerRadius;
            _gradient.SetCornerRadii(new float[] { cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius });

            /* ==================================================================================================
             * finish up
             * ================================================================================================*/
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                /* ==================================================================================================
                 * keep the built-in effect of button
                 * ================================================================================================*/
                var bg = Control.Background as RippleDrawable;
                int[][] states =
                {
                    new int[] { Android.Resource.Attribute.StateEnabled}, // enabled
                    new int[] { Android.Resource.Attribute.OnClick}, // hover
                    new int[] { -Android.Resource.Attribute.StateEnabled}, // Disabled
                };
                var colors = new int[] { Android.Graphics.Color.LightGray, Android.Graphics.Color.DarkGray, Android.Graphics.Color.DarkGray };
                bg.SetColor(new ColorStateList(states, colors));
                if (bg.NumberOfLayers < 2)
                    bg?.AddLayer(_gradient);
                Control.SetBackground(bg);
            }
            else
            {
                // todo: keep base hightlight effect
                Control?.SetBackground(_gradient);
                lock (_lockObj)
                {
                    if (!_isEventSet)
                    {
                        //Control.Touch += Control_Touch;
                        _isEventSet = true;
                    }
                }
            }
        }

        private async void OnControlClick(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is AppCompatButton btn) || !(btn.Background is GradientDrawable gradient))
                {
                    //var extendedButton = Element as ExtendedButton;
                    //if (extendedButton?.Command?.CanExecute(extendedButton.CommandParameter) ?? false)
                    //extendedButton?.Command?.Execute(extendedButton?.CommandParameter);
                    return;
                }
                //Xamarin.Forms.Platform.Android.AppCompat.ButtonRenderer.IOnCl

                var alpha = gradient.Alpha;
                gradient.SetAlpha(180);
                await Task.Delay(100);
                gradient.SetAlpha(alpha);
            }
            catch (ObjectDisposedException ex)
            {
                /* ==================================================================================================
                 * in case force click/tap quickly, this instance maybe disposed before this method done => ignore
                 * ================================================================================================*/
                LogCommon.Error(ex);
            }
            catch (Exception ex)
            {
                LogCommon.Error(ex);
                throw ex;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && _isEventSet && Control != null)
                Control.Click -= OnControlClick;
        }
    }


}
