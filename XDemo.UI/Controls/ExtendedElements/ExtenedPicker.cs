using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using Xamarin.Forms;
using XDemo.Core.Infrastructure.Logging;

namespace XDemo.UI.Controls.ExtendedElements
{
    public class ExtendedPicker : Picker
    {
        bool _disableNestedCall;

        public ExtendedPicker()
        {
            SelectedIndexChanged += OnSelectedIndexChanged;
        }

        ~ExtendedPicker()
        {
            SelectedIndexChanged -= OnSelectedIndexChanged;
        }

        #region ItemsSource
        public new static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(ExtendedPicker),
                null, propertyChanged: OnItemsSourceChanged);

        public new IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                if (newValue == null && oldValue == null)
                    return;

                var picker = bindable as ExtendedPicker;
                picker?.InstanceOnItemsSourceChanged(oldValue, newValue);
            }
            catch (ObjectDisposedException ex)
            {
                // ignore this exception
                LogCommon.Error(ex);
            }
        }
        #endregion

        #region SelectedItem
        public new static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(ExtendedPicker),
                null, BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

        public new object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                var boundPicker = bindable as ExtendedPicker;
                boundPicker?.InternalSelectedItemChanged();
                boundPicker?.ItemSelected?.Invoke(boundPicker, new SelectedItemChangedEventArgs(newValue));
            }
            catch (ObjectDisposedException ex)
            {
                // ignore this exception
                LogCommon.Error(ex);
            }
        }
        #endregion

        #region SelectedValue
        public static readonly BindableProperty SelectedValueProperty =
            BindableProperty.Create(nameof(SelectedValue), typeof(object), typeof(ExtendedPicker),
                null, BindingMode.TwoWay, propertyChanged: OnSelectedValueChanged);

        public object SelectedValue
        {
            get => GetValue(SelectedValueProperty);
            set
            {
                SetValue(SelectedValueProperty, value);
                InternalSelectedValueChanged();
            }
        }

        static void OnSelectedValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                var boundPicker = bindable as ExtendedPicker;
                boundPicker?.InternalSelectedValueChanged();
            }
            catch (ObjectDisposedException ex)
            {
                // ignore this exception
                LogCommon.Error(ex);
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets the placeholder.
        /// </summary>
        /// <value>The placeholder.</value>
        public string Placeholder { get; set; }

        /// <summary>
        /// Gets or sets the display member path.
        /// </summary>
        /// <value>The display member path.</value>
        public string DisplayMemberPath { get; set; }

        /// <summary>
        /// Gets or sets the selected value path.
        /// </summary>
        /// <value>The selected value path.</value>
        public string SelectedValuePath { get; set; }

        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;

        void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_disableNestedCall)
            {
                return;
            }

            if (SelectedIndex < 0 || ItemsSource == null || !ItemsSource.GetEnumerator().MoveNext())
            {
                _disableNestedCall = true;
                SelectedIndex = -1;
                SelectedItem = null;
                SelectedValue = null;
                _disableNestedCall = false;
                return;
            }

            _disableNestedCall = true;
            var index = 0;
            var hasSelectedValuePath = !string.IsNullOrWhiteSpace(SelectedValuePath);
            foreach (var item in ItemsSource)
            {
                if (index != SelectedIndex)
                {
                    index++;
                    continue;
                }

                SelectedItem = item;
                if (hasSelectedValuePath)
                {
                    var prop = item.GetType().GetRuntimeProperty(SelectedValuePath);
                    SelectedValue = prop?.GetValue(item);
                }

                break;
            }

            _disableNestedCall = false;
        }

        void InstanceOnItemsSourceChanged(object oldValue, object newValue)
        {
            _disableNestedCall = true;
            Items?.Clear();
            if (oldValue is INotifyCollectionChanged oldCollectionINotifyCollectionChanged)
            {
                oldCollectionINotifyCollectionChanged.CollectionChanged -= OnItemsSourceCollectionChanged;
            }

            if (newValue is INotifyCollectionChanged newCollectionINotifyCollectionChanged)
            {
                newCollectionINotifyCollectionChanged.CollectionChanged += OnItemsSourceCollectionChanged;
            }

            if (newValue != null)
            {
                var hasDisplayMemberPath = !string.IsNullOrWhiteSpace(DisplayMemberPath);

                foreach (var item in (IEnumerable)newValue)
                {
                    if (hasDisplayMemberPath)
                    {
                        var prop = item.GetType().GetRuntimeProperty(DisplayMemberPath);
                        Items.Add(prop?.GetValue(item)?.ToString());
                    }
                    else
                    {
                        Items.Add(item?.ToString());
                    }
                }

                SelectedIndex = -1;
                _disableNestedCall = false;

                if (SelectedItem != null)
                {
                    InternalSelectedItemChanged();
                }
                else if (hasDisplayMemberPath && SelectedValue != null)
                {
                    InternalSelectedValueChanged();
                }
            }
            else
            {
                _disableNestedCall = true;
                SelectedIndex = -1;
                SelectedItem = null;
                SelectedValue = null;
                _disableNestedCall = false;
            }
        }

        void InternalSelectedItemChanged()
        {
            if (_disableNestedCall)
            {
                return;
            }

            var selectedIndex = -1;
            object selectedValue = null;
            if (ItemsSource != null)
            {
                var index = 0;
                var hasSelectedValuePath = !string.IsNullOrWhiteSpace(SelectedValuePath);
                foreach (var item in ItemsSource)
                {
                    if (item != null && item.Equals(SelectedItem))
                    {
                        selectedIndex = index;
                        if (hasSelectedValuePath)
                        {
                            var prop = item.GetType().GetRuntimeProperty(SelectedValuePath);
                            selectedValue = prop.GetValue(item);
                        }
                        break;
                    }
                    index++;
                }
            }
            _disableNestedCall = true;
            SelectedValue = selectedValue;
            SelectedIndex = selectedIndex;
            _disableNestedCall = false;
        }

        void InternalSelectedValueChanged()
        {
            if (_disableNestedCall)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedValuePath))
            {
                return;
            }
            var selectedIndex = -1;
            object selectedItem = null;
            if (ItemsSource != null)
            {
                var index = 0;
                foreach (var item in ItemsSource)
                {
                    if (item == null)
                    {
                        index++;
                        continue;
                    }

                    var type = item.GetType();
                    var prop = type.GetRuntimeProperty(SelectedValuePath);
                    if (Equals(prop.GetValue(item), SelectedValue))
                    {
                        selectedIndex = index;
                        selectedItem = item;
                        break;
                    }
                }
            }

            _disableNestedCall = true;
            SelectedItem = selectedItem;
            SelectedIndex = selectedIndex;
            _disableNestedCall = false;
        }

        void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var hasDisplayMemberPath = !string.IsNullOrWhiteSpace(DisplayMemberPath);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        if (hasDisplayMemberPath)
                        {
                            var prop = item.GetType().GetRuntimeProperty(DisplayMemberPath);
                            Items.Add(prop?.GetValue(item)?.ToString());
                        }
                        else
                        {
                            Items.Add(item?.ToString());
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.NewItems)
                    {
                        if (hasDisplayMemberPath)
                        {
                            var prop = item.GetType().GetRuntimeProperty(DisplayMemberPath);
                            Items.Remove(prop.GetValue(item)?.ToString());
                        }
                        else
                        {
                            Items.Remove(item?.ToString());
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.NewItems)
                    {
                        if (hasDisplayMemberPath)
                        {
                            var prop = item.GetType().GetRuntimeProperty(DisplayMemberPath);
                            Items.Remove(prop.GetValue(item).ToString());
                        }
                        else
                        {
                            var index = Items.IndexOf(item?.ToString());
                            if (index > -1)
                            {
                                Items[index] = item.ToString();
                            }
                        }
                    }
                    break;
            }
        }
    }
}
