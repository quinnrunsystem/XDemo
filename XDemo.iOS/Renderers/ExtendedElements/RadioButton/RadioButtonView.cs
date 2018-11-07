﻿using CoreGraphics;
using Foundation;
using UIKit;

namespace XDemo.iOS.Renderers.ExtendedElements.RadioButton
{
    [Register(nameof(RadioButtonView))]
    public class RadioButtonView : UIButton
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonView"/> class.
        /// </summary>
        public RadioButtonView()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonView"/> class.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        public RadioButtonView(CGRect bounds)
            : base(bounds)
        {
            Initialize();
        }


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadioButtonView"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        public bool Checked
        {
            set => Selected = value;
            get => Selected;
        }

        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            set => SetTitle(value, UIControlState.Normal);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize()
        {
            AdjustEdgeInsets();
            ApplyStyle();

            // set default color, because type is not UIButtonType.System 
            SetTitleColor(UIColor.DarkTextColor, UIControlState.Normal);
            SetTitleColor(UIColor.DarkTextColor, UIControlState.Selected);
            TouchUpInside += (sender, args) => Selected = true;
        }

        /// <summary>
        /// Adjusts the edge insets.
        /// </summary>
        void AdjustEdgeInsets()
        {
            const float inset = 8f;

            HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            ImageEdgeInsets = new UIEdgeInsets(0f, inset, 0f, 0f);
            TitleEdgeInsets = new UIEdgeInsets(0f, inset * 2, 0f, 0f);
        }

        /// <summary>
        /// Applies the style.
        /// </summary>
        void ApplyStyle()
        {
            var imgChecked = UIImage.FromFile("checked");
            var imgUnChecked = UIImage.FromFile("unchecked");
            SetImage(imgChecked, UIControlState.Selected);
            SetImage(imgUnChecked, UIControlState.Normal);
        }
    }
}
