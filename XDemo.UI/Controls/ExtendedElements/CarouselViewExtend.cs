using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XDemo.Core.Infrastructure.Logging;

namespace XDemo.UI.Controls.ExtendedElements
{
    public class CarouselViewExtend : ScrollView
    {
        //StackLayout _stack;

        int _selectedIndex;

        public CarouselViewExtend()
        {
            Orientation = ScrollOrientation.Horizontal;

            //_stack = new StackLayout
            //{
            //    Orientation = StackOrientation.Horizontal,
            //    Spacing = 0
            //};

            //Content = _stack;
        }


        public IList<View> ChildrenList
        {
            get
            {
                if (Content is StackLayout st)
                {
                    return st.Children;
                }
                else
                {
                    return new List<View>();
                }
                //return Content is StackLayout st ? st.Children : null;
                //return _stack.Children;
            }
        }

        private bool _layingOutChildren;
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);
            if (_layingOutChildren) return;

            _layingOutChildren = true;
            foreach (var child in ChildrenList) child.WidthRequest = width;
            _layingOutChildren = false;
        }

        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create(
                nameof(SelectedIndex),
                typeof(int),
                typeof(CarouselViewExtend),
                0,
                BindingMode.TwoWay,
                propertyChanged: async (bindable, oldValue, newValue) =>
                {
                    await ((CarouselViewExtend)bindable).UpdateSelectedItem();
                }
            );

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        async Task UpdateSelectedItem()
        {
            await Task.Delay(300);

            if (-1 < SelectedIndex)
            {
                //if (Content is StackLayout st)
                //{
                //    SelectedItem = st.Children[SelectedIndex].BindingContext;
                //}



                if (SelectedIndex == ChildrenList.Count)
                {
                    SelectedItem = ChildrenList[ChildrenList.Count - 1].BindingContext;
                }
                else if(SelectedIndex == 1)
                {
                    SelectedItem = ChildrenList[0].BindingContext;
                }
                else
                {
                    SelectedItem = ChildrenList[SelectedIndex].BindingContext;
                }

                //await ScrollToAsync(ChildrenList[SelectedIndex], ScrollToPosition.Start, false);
            }
            LogCommon.Info("SelectedIndex: " + SelectedIndex);
        }

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(
                nameof(SelectedItem),
                typeof(object),
                typeof(CarouselViewExtend),
                null,
                BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((CarouselViewExtend)bindable).UpdateSelectedIndex();
                }
            );

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        void UpdateSelectedIndex()
        {
            if (SelectedItem == BindingContext) return;

            SelectedIndex = ChildrenList
                .Select(c => c.BindingContext)
                .ToList()
                .IndexOf(SelectedItem);
        }

        #region Spacing Item
        public static readonly BindableProperty SpacingItemsProperty =
            BindableProperty.Create(nameof(SpacingItems), typeof(double), typeof(CarouselViewExtend), 0.0d);
        public double SpacingItems
        {
            get { return (double)GetValue(SpacingItemsProperty); }
            set { SetValue(SpacingItemsProperty, value); }
        }
        #endregion

        #region --For items source--
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(CarouselViewExtend), default(IList),
                                    BindingMode.Default, null, 
                                    new BindableProperty.BindingPropertyChangedDelegate(HandleBindingPropertyChangedDelegate), HandleBindingPropertyChangingDelegate);

        static void HandleBindingPropertyChangingDelegate(BindableObject bindable, object oldValue, object newValue)
        {
            var tl = (CarouselViewExtend)bindable;
            if (tl.ItemsSource == null) return;
            tl._selectedIndex = tl.ItemsSource.IndexOf(tl.SelectedItem);
        }


        static void HandleBindingPropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue)
        {
            var isOldObservable = oldValue?.GetType().GetTypeInfo().ImplementedInterfaces.Any(i => i == typeof(INotifyCollectionChanged));
            var isNewObservable = newValue?.GetType().GetTypeInfo().ImplementedInterfaces.Any(i => i == typeof(INotifyCollectionChanged));

            var tl = (CarouselViewExtend)bindable;
            if (isOldObservable.GetValueOrDefault(false))
            {
                ((INotifyCollectionChanged)oldValue).CollectionChanged -= tl.HandleCollectionChanged;
            }

            if (isNewObservable.GetValueOrDefault(false))
            {
                ((INotifyCollectionChanged)newValue).CollectionChanged += tl.HandleCollectionChanged;
            }

            if (oldValue != newValue)
            {
                tl.Render();
            }
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Render();
        }

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        #endregion

        #region -- For item template --
        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(CarouselViewExtend), default(DataTemplate));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        #endregion

        #region -- For item selected --
        public event EventHandler<ItemTappedEventArgs> ItemSelected;

        public static readonly BindableProperty SelectedCommandProperty =
            BindableProperty.Create(nameof(SelectedCommand), typeof(ICommand), typeof(CarouselViewExtend), null);

        public ICommand SelectedCommand
        {
            get { return (ICommand)GetValue(SelectedCommandProperty); }
            set { SetValue(SelectedCommandProperty, value); }
        }

        public static readonly BindableProperty SelectedCommandParameterProperty =
            BindableProperty.Create(nameof(SelectedCommandParameter), typeof(object), typeof(CarouselViewExtend), null);

        public object SelectedCommandParameter
        {
            get { return GetValue(SelectedCommandParameterProperty); }
            set { SetValue(SelectedCommandParameterProperty, value); }
        }
        #endregion

        /// <summary>
        /// Render this instance.
        /// </summary>
        public void Render()
        {
            if (ItemTemplate == null || ItemsSource == null)
            {
                Content = null;
                return;
            }

            var layout = new StackLayout();
            layout.Spacing = SpacingItems;
            layout.Orientation = Orientation == ScrollOrientation.Vertical ? StackOrientation.Vertical : StackOrientation.Horizontal;

            foreach (var item in ItemsSource)
            {
                var command = SelectedCommand ?? new Command((obj) =>
                {
                    var args = new ItemTappedEventArgs(ItemsSource, item);
                    ItemSelected?.Invoke(this, args);
                });
                var commandParameter = SelectedCommandParameter ?? item;

                var viewCell = ItemTemplate.CreateContent() as ViewCell;
                viewCell.View.BindingContext = item;
                viewCell.View.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = command,
                    CommandParameter = commandParameter,
                    NumberOfTapsRequired = 1
                });

                layout.Children.Add(viewCell.View);
            }

            if (_selectedIndex >= 0) SelectedIndex = _selectedIndex;

            Device.BeginInvokeOnMainThread(() =>
            {
                Content = layout;
            });
        }
    }
}
