using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace XDemo.UI.Controls.ExtendedElements
{
    /// <summary>
    /// The check box.
    /// </summary>
    public class CheckBox : View
    {
        #region Checked property

        public static readonly BindableProperty CheckedProperty = BindableProperty.Create(nameof(Checked), typeof(bool), typeof(CheckBox), default(bool), BindingMode.TwoWay, propertyChanged: OnCheckedChanged);

        private static void OnCheckedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var checkBox = (CheckBox)bindable;
            checkBox.Checked = (newValue as bool?) ?? false;
        }

        public bool Checked
        {
            get => (bool)GetValue(CheckedProperty);
            set
            {
                var valueChanged = Checked != value;
                SetValue(CheckedProperty, value);
                if (valueChanged)
                    CheckedChanged?.Invoke(this, new EventArg<bool>(value));
            }
        }

        #endregion

        #region UncheckedText property

        public static readonly BindableProperty UncheckedTextProperty = BindableProperty.Create(nameof(UncheckedText), typeof(string), typeof(CheckBox), default(string), BindingMode.TwoWay);

        public string UncheckedText
        {
            get => (string)GetValue(UncheckedTextProperty);
            set => SetValue(UncheckedTextProperty, value);
        }

        #endregion

        #region CheckedText property

        public static readonly BindableProperty CheckedTextProperty = BindableProperty.Create(nameof(CheckedText), typeof(string), typeof(CheckBox), default(string), BindingMode.TwoWay);

        public string CheckedText
        {
            get => (string)GetValue(CheckedTextProperty);
            set => SetValue(CheckedTextProperty, value);
        }

        #endregion

        #region DefaultText property

        public static readonly BindableProperty DefaultTextProperty = BindableProperty.Create(nameof(DefaultText), typeof(string), typeof(CheckBox), default(string), BindingMode.TwoWay);

        public string DefaultText
        {
            get => (string)GetValue(DefaultTextProperty);
            set => SetValue(DefaultTextProperty, value);
        }

        #endregion

        #region TextColor property

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CheckBox), Color.Black, BindingMode.TwoWay);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        #endregion

        #region FontSize property

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(CheckBox), -1, BindingMode.TwoWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        #endregion

        #region FontName property

        public static readonly BindableProperty FontNameProperty = BindableProperty.Create(nameof(FontName), typeof(string), typeof(CheckBox), default(string), BindingMode.TwoWay);

        public string FontName
        {
            get => (string)GetValue(FontNameProperty);
            set => SetValue(FontNameProperty, value);
        }

        #endregion

        /// <summary>
        /// The checked changed event.
        /// </summary>
        public event EventHandler<EventArg<bool>> CheckedChanged;

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text => Checked
                    ? (string.IsNullOrEmpty(CheckedText) ? DefaultText : CheckedText)
                        : (string.IsNullOrEmpty(UncheckedText) ? DefaultText : UncheckedText);
    }
}