using System;
using System.ComponentModel;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XDemo.iOS.Extensions;
using XDemo.iOS.Renderers.ExtendedElements.RadioButton;
using XDemo.UI.Controls.ExtendedElements.RadioButton;

[assembly: ExportRenderer(typeof(RadioButton), typeof(RadioButtonRenderer))]

namespace XDemo.iOS.Renderers.ExtendedElements.RadioButton
{
    /// <summary>
    /// The Radio button renderer for iOS.
    /// </summary>
    public class RadioButtonRenderer : ViewRenderer<UI.Controls.ExtendedElements.RadioButton.RadioButton, RadioButtonView>
    {
        private UIColor _defaultTextColor;

        /// <summary>
        /// Handles the Element Changed event
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<UI.Controls.ExtendedElements.RadioButton.RadioButton> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var checkBox = new RadioButtonView(Bounds);
                _defaultTextColor = checkBox.TitleColor(UIControlState.Normal);

                checkBox.TouchUpInside += (s, args) => Element.Checked = Control.Checked;

                SetNativeControl(checkBox);
            }

            if (Element == null) return;

            BackgroundColor = Element.BackgroundColor.ToUIColor();
            UpdateFont();

            Control.LineBreakMode = UILineBreakMode.WordWrap;
            Control.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            Control.Text = Element.Text;
            Control.Checked = Element.Checked;
            UpdateTextColor();
        }

        /// <summary>
        /// Resizes the text.
        /// </summary>
        private void ResizeText()
        {
            var text = Element.Text;

            var bounds = Control.Bounds;

            var width = Control.TitleLabel.Bounds.Width;

            var imgChecked = UIImage.FromFile("checked");

            nfloat height;
            if (text.StringHeight(Control.Font, width) < imgChecked.Size.Height)
            {
               height = imgChecked.Size.Height;
            }
            else
            {
                height = text.StringHeight(Control.Font, width);
            }
            var minHeight = string.Empty.StringHeight(Control.Font, width);

            var requiredLines = Math.Round(height / minHeight, MidpointRounding.AwayFromZero);

            var supportedLines = Math.Round(bounds.Height / minHeight, MidpointRounding.ToEven);

            if (supportedLines == requiredLines)
            {
                return;
            }

            bounds.Height += (float)(minHeight * (requiredLines - supportedLines));

            Control.Bounds = bounds;
            Element.HeightRequest = bounds.Height;
        }

        /// <summary>
        /// Draws the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            ResizeText();
        }

        /// <summary>
        /// Updates the font.
        /// </summary>
        private void UpdateFont()
        {
            if (string.IsNullOrEmpty(Element.FontName))
            {
                return;
            }

            var font = UIFont.FromName(Element.FontName, (Element.FontSize > 0) ? (float)Element.FontSize : 12.0f);

            if (font != null)
            {
                Control.Font = font;
            }
        }

        /// <summary>
        /// Updates the color of the text.
        /// </summary>
        private void UpdateTextColor()
        {
            Control.SetTitleColor(Element.TextColor.ToUIColorOrDefault(_defaultTextColor), UIControlState.Normal);
            Control.SetTitleColor(Element.TextColor.ToUIColorOrDefault(_defaultTextColor), UIControlState.Selected);
        }

        /// <summary>
        /// Handles the <see cref="E:ElementPropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            switch (e.PropertyName)
            {
                case nameof(UI.Controls.ExtendedElements.RadioButton.RadioButton.Checked):
                    Control.Checked = Element.Checked;
                    break;
                case nameof(UI.Controls.ExtendedElements.RadioButton.RadioButton.Text):
                    Control.Text = Element.Text;
                    break;
                case nameof(UI.Controls.ExtendedElements.RadioButton.RadioButton.TextColor):
                    UpdateTextColor();
                    break;
                case nameof(Element):
                    // do nothing
                    break;
                case nameof(UI.Controls.ExtendedElements.RadioButton.RadioButton.FontSize):
                    UpdateFont();
                    break;
                case nameof(UI.Controls.ExtendedElements.RadioButton.RadioButton.FontName):
                    UpdateFont();
                    break;
                default:
                    //                    Debug.WriteLine("Property change for {0} has not been implemented.", e.PropertyName);
                    return;
            }
        }
    }
}
