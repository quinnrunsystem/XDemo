using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace XDemo.UI.Controls.ExtendedElements.RadioButton
{
    /// <summary>
    /// Class CustomRadioButton.
    /// </summary>
    public class RadioButton : View
    {
        /// <summary>
        /// Gets or sets the RAD identifier. <para/>
        /// for RadioGroup internal used
        /// </summary>
        /// <value>The RAD identifier.</value>
        public int RadId { get; set; }

        public event EventHandler<EventArg<bool>> CheckedChanged;

        #region Checked property

        public static readonly BindableProperty CheckedProperty = BindableProperty.Create(nameof(Checked), typeof(bool), typeof(RadioButton), default(bool));
        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="T:XDemo.UI.Controls.ExtendedElements.RadioButton.RadioButton"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        public bool Checked
        {
            get => (bool)GetValue(CheckedProperty);
            set
            {
                /* ==================================================================================================
                 * predicate changed
                 * ================================================================================================*/
                var valueChanged = Checked != value;
                SetValue(CheckedProperty, value);
                /* ==================================================================================================
                 * raise checked change event AFTER value set
                 * ================================================================================================*/
                if (valueChanged)
                    CheckedChanged?.Invoke(this, new EventArg<bool>(value));
            }
        }

        #endregion

        #region Text property

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(RadioButton), default(string));

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        #endregion

        #region TextColor property

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(RadioButton), Color.Default);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        #endregion

        #region FontSize property

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(RadioButton), -1);
        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        #endregion

        #region FontName property

        public static readonly BindableProperty FontNameProperty = BindableProperty.Create(nameof(FontName), typeof(string), typeof(RadioButton), default(string));
        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        /// <value>The name of the font.</value>
        public string FontName
        {
            get => (string)GetValue(FontNameProperty);
            set => SetValue(FontNameProperty, value);
        }

        #endregion
    }
}
