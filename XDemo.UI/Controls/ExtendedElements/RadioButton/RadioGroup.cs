using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace XDemo.UI.Controls.ExtendedElements.RadioButton
{
    /// <summary>
    /// Class BindableRadioGroup.
    /// </summary>
    public class RadioGroup : StackLayout
    {
        /// <summary>
        /// The items
        /// </summary>
        public ObservableCollection<RadioButton> Items;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioGroup"/> class.
        /// </summary>
        public RadioGroup()
        {
            Items = new ObservableCollection<RadioButton>();
        }

        /// <summary>
        /// The items source property
        /// </summary>
        #region ItemsSource property

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(RadioGroup), default(IEnumerable), BindingMode.TwoWay, propertyChanged: OnItemsSourceChanged);

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var radButtons = bindable as RadioGroup;
            foreach (var item in radButtons.Items)
            {
                item.CheckedChanged -= radButtons.OnGroupCheckedChanged;
            }

            radButtons.Children.Clear();

            var radIndex = 0;

            foreach (var item in radButtons.ItemsSource)
            {
                var button = new RadioButton
                {
                    Text = item.ToString(),
                    RadId = radIndex++,
                    TextColor = radButtons.TextColor,
                    //FontSize = Device.GetNamedSize(NamedSize.Small, radButtons),
                    FontSize = radButtons.FontSize,
                    FontName = radButtons.FontName
                };

                button.CheckedChanged += radButtons.OnGroupCheckedChanged;
                radButtons.Items.Add(button);
                radButtons.Children.Add(button);
            }
        }
        #endregion


        /// <summary>
        /// The selected index property
        /// </summary>
        #region SelectedIndex property

        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(RadioGroup), -1, BindingMode.TwoWay, propertyChanged: OnSelectedIndexChanged);

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var val = newValue as int?;
            if (!val.HasValue || val.Value < 0)
                return;

            if (!(bindable is RadioGroup bindableRadioGroup))
                return;

            foreach (var button in bindableRadioGroup.Items.Where(button => button.RadId == bindableRadioGroup.SelectedIndex))
            {
                button.Checked = true;
            }
        }

        #endregion

        #region TextColor property

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(RadioGroup), Color.Black, BindingMode.TwoWay);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        #endregion

        #region FontSize property

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(RadioGroup), -1, BindingMode.TwoWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        #endregion

        #region FontName property

        public static readonly BindableProperty FontNameProperty = BindableProperty.Create(nameof(FontName), typeof(string), typeof(RadioGroup), default(string), BindingMode.TwoWay);

        public string FontName
        {
            get => (string)GetValue(FontNameProperty);
            set => SetValue(FontNameProperty, value);
        }

        #endregion

        /// <summary>
        /// Occurs when [checked changed].
        /// </summary>
        public event EventHandler<EventArg<int>> CheckedChanged;

        private void OnGroupCheckedChanged(object sender, EventArg<bool> e)
        {
            if (e.Data == false)
                return;

            if (!(sender is RadioButton selectedRadButton))
                return;

            foreach (var item in Items)
            {
                if (!selectedRadButton.RadId.Equals(item.RadId))
                {
                    item.Checked = false;
                }
                else
                {
                    if (SelectedIndex != selectedRadButton.RadId)
                    {
                        CheckedChanged?.Invoke(sender, new EventArg<int>(item.RadId));
                    }
                    SelectedIndex = selectedRadButton.RadId;
                }
            }
        }
    }
}
