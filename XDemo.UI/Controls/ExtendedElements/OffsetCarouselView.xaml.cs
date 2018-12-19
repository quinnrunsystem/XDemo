using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace XDemo.UI.Controls.ExtendedElements
{
    public class HorizontalScrollView : ScrollView
    {
        public HorizontalScrollView()
        {
            Orientation = ScrollOrientation.Horizontal;
        }
    }
    public partial class OffsetCarouselView : ContentView
    {
        public event EventHandler<CarouselViewSelectedItemChangedEventArgs> ItemSelected;
        public class CarouselViewSelectedItemChangedEventArgs
        {
            public CarouselViewSelectedItemChangedEventArgs(int selectedIndex, object selectedObject)
            {
                SelectedIndex = selectedIndex;
                SelectedObject = selectedObject;
            }
            public int SelectedIndex { get; private set; }
            public object SelectedObject { get; private set; }
        }
        #region Precision
        const double _4 = 0.0001;
        const double _5 = 0.00001;
        const double _6 = 0.000001;
        const double _7 = 0.0000001;
        const double _8 = 0.00000001;
        const double _9 = 0.000000001;
        /// <summary>
        /// Compare 2 double numbers with desired precision
        /// </summary>
        /// <returns><c>true</c>, if digit precision was equals9ed, <c>false</c> otherwise.</returns>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        static bool Equals9DigitPrecision(double left, double right)
        {
            return Math.Abs(left - right) < _9;
        }
        #endregion
        /// <summary>
        /// Mark initialized for propertyChanged event to check its logics
        /// </summary>
        bool _init;
        /// <summary>
        /// Changes from these properties will fire render action
        /// </summary>
        static string[] _rerenderProperty =
        {
            nameof(ItemSource),
            nameof(OffsetPercent),
            nameof(ItemTemplate),
        };
        Grid _activeIndicatorContainer;
        public OffsetCarouselView()
        {
            InitializeComponent();
            _init = true;
            InitActiveIndicator();
            Render();
            scroll.Scrolled += Scroll_Scrolled;
        }

        void Scroll_Scrolled(object sender, ScrolledEventArgs e)
        {
            SetIndicatorPosition();
        }
        void SetIndicatorPosition()
        {
            GetIndexFromScrollX(out int indexInt, out double indexRemain);
            var index = indexInt + indexRemain;
            var leftMargin = index * IndicatorWidth;
            //fix on last item
            var lastSnapX = (stack.Children.Count - 2) * ItemWidth - OffsetWidth;
            if (Equals9DigitPrecision(scroll.ScrollX, lastSnapX))
                leftMargin = (stackIndicator.Children.Count - 1) * IndicatorWidth;
            ActiveIndicatorPosition = new Thickness { Left = leftMargin };
        }

        void InitActiveIndicator()
        {
            var dot = new BoxView
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            dot.SetBinding(WidthRequestProperty, new Binding
            {
                Source = this,
                Path = nameof(IndicatorSize),
            });
            dot.SetBinding(HeightRequestProperty, new Binding
            {
                Source = this,
                Path = nameof(IndicatorSize),
            });
            dot.SetBinding(BackgroundColorProperty, new Binding
            {
                Source = this,
                Path = nameof(IndicatorActiveColor),
            });
            dot.SetBinding(BoxView.CornerRadiusProperty, new Binding
            {
                Source = this,
                Path = nameof(IndicatorRadius),
            });
            var dotContainer = new Grid
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };
            dotContainer.SetBinding(WidthRequestProperty, new Binding
            {
                Source = this,
                Path = nameof(IndicatorWidth),
            });
            dotContainer.SetBinding(MarginProperty, new Binding
            {
                Source = this,
                Path = nameof(ActiveIndicatorPosition),
            });
            dotContainer.Children.Add(dot);
            _activeIndicatorContainer = dotContainer;
        }
        private Thickness _activeIndicatorPosition = new Thickness();
        public Thickness ActiveIndicatorPosition
        {
            get => _activeIndicatorPosition; private set
            {
                _activeIndicatorPosition = value;
                OnPropertyChanged();
            }
        }

        private double _itemWidth;

        /// <summary>
        /// Width of item
        /// </summary>
        /// <value>The width of the item.</value>
        public double ItemWidth
        {
            get => _itemWidth; private set
            {
                _itemWidth = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OffsetWidth));
            }
        }
        /// <summary>
        /// Width calculated from offset percent
        /// </summary>
        /// <value>The width of the offset.</value>
        public double OffsetWidth => Width - ItemWidth;
        /// <summary>
        /// Percent of control's width for this carousel view
        /// </summary>
        /// <value>The offset percent.</value>
        public double OffsetPercent
        {
            get { return (double)GetValue(OffsetPercentProperty); }
            set { SetValue(OffsetPercentProperty, value); }
        }
        public static readonly BindableProperty OffsetPercentProperty =
  BindableProperty.Create(nameof(OffsetPercent), typeof(double), typeof(OffsetCarouselView), 0.2);

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        public static readonly BindableProperty ItemTemplateProperty =
  BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(OffsetCarouselView), null);

        public IEnumerable ItemSource
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }
        public static readonly BindableProperty ItemSourceProperty =
  BindableProperty.Create(nameof(ItemSource), typeof(IEnumerable), typeof(OffsetCarouselView), null);


        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
        public static readonly BindableProperty SelectedIndexProperty =
  BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(OffsetCarouselView), -1, propertyChanged: SelectedIndexChanged);

        public Color IndicatorColor
        {
            get { return (Color)GetValue(IndicatorColorProperty); }
            set { SetValue(IndicatorColorProperty, value); }
        }
        public static readonly BindableProperty IndicatorColorProperty =
  BindableProperty.Create(nameof(IndicatorColor), typeof(Color), typeof(OffsetCarouselView), Color.White);
        public Color IndicatorActiveColor
        {
            get { return (Color)GetValue(IndicatorActiveColorProperty); }
            set { SetValue(IndicatorActiveColorProperty, value); }
        }
        public static readonly BindableProperty IndicatorActiveColorProperty =
  BindableProperty.Create(nameof(IndicatorColor), typeof(Color), typeof(OffsetCarouselView), Color.Gray);
        public double IndicatorWidth
        {
            get { return (double)GetValue(IndicatorWidthProperty); }
            set { SetValue(IndicatorWidthProperty, value); }
        }
        public static readonly BindableProperty IndicatorWidthProperty =
  BindableProperty.Create(nameof(IndicatorWidth), typeof(double), typeof(OffsetCarouselView), 16.0);
        public double IndicatorSize
        {
            get { return (double)GetValue(IndicatorSizeProperty); }
            set { SetValue(IndicatorSizeProperty, value); }
        }
        public static readonly BindableProperty IndicatorSizeProperty =
  BindableProperty.Create(nameof(IndicatorSize), typeof(double), typeof(OffsetCarouselView), 8.0, propertyChanged: IndicatorSizeChanged);

        private static void IndicatorSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is OffsetCarouselView carouselView && newValue is int)
                carouselView.OnPropertyChanged(nameof(IndicatorRadius));
        }

        public double IndicatorRadius => IndicatorSize / 2.0;
        public bool IsIndicatorVisible
        {
            get { return (bool)GetValue(IsIndicatorVisibleProperty); }
            set { SetValue(IsIndicatorVisibleProperty, value); }
        }
        public static readonly BindableProperty IsIndicatorVisibleProperty =
  BindableProperty.Create(nameof(IsIndicatorVisible), typeof(bool), typeof(OffsetCarouselView), true);

        public object ScrollToObject
        {
            get
            {
                if (SelectedIndex == -1) return null;
                var selectedObj = ItemSource?.OfType<object>()?.ElementAt(SelectedIndex);
                return selectedObj;
            }
        }
        private object _selectedObject;

        public object SelectedObject
        {
            get => _selectedObject;
            private set
            {
                _selectedObject = value;
                OnPropertyChanged();
            }
        }

        private static void SelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is OffsetCarouselView carouselView && newValue is int newNum)
            {
                carouselView.OnPropertyChanged(nameof(carouselView.ScrollToObject));
                var needScroll = carouselView.VerifyScrollOnIndexChanged();
                if (needScroll) carouselView.ScrollToIndex(newNum);
            }
        }
        bool VerifyScrollOnIndexChanged()
        {
            var requiredScroll = SelectedIndex != IndexFromScrollX;
            return requiredScroll;
        }
        /// <summary>
        /// Call this property in native to snap item into index
        /// </summary>
        public void SnapHandler()
        {
            GetIndexFromScrollX(out int indexInt, out double indexRemain);
            var snapAdd = Math.Round(indexRemain, 0);
            var snapItem = indexInt + (int)snapAdd;
            ScrollToIndex(snapItem);
        }
        /// <summary>
        /// Scroll to specific index
        /// </summary>
        /// <param name="index">Index.</param>
        public void ScrollToIndex(int index)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var snapX = index * ItemWidth;
                if (index == stack.Children.Count - 2) //snap last item to the right
                    snapX -= OffsetWidth;
                try
                {
                    await scroll.ScrollToAsync(snapX, scroll.ScrollY, true);
                    SelectedIndex = index; //set new selected index
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            });
        }
        /// <summary>
        /// Index calculated from scroll X. It should be the same as SelectedIndex
        /// </summary>
        /// <value>The index from scroll x.</value>
        public int IndexFromScrollX
        {
            get
            {
                GetIndexFromScrollX(out int indexInt, out double indexRemain);
                return indexInt;
            }
        }

        void GetIndexFromScrollX(out int indexInt, out double indexRemain)
        {
            var currentX = scroll.ScrollX;
            var itemSize = ItemWidth;
            var currentIndex = currentX / itemSize;
            indexInt = (int)currentIndex;
            indexRemain = currentIndex - indexInt;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (_init)
            {
                if (_rerenderProperty.Contains(propertyName))
                {
                    Render();
                }
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            //Calculate size for item
            if (width > 0 && height > 0) ItemWidth = width * (1.0 - OffsetPercent);
            base.OnSizeAllocated(width, height);
        }


        void Render()
        {
            ClearOldRender();
            var notRender = VerifyNotRender();
            if (notRender) return;
            RenderCarousel();
            RenderIndicator();
            //set selected index
            SelectedIndex = stack.Children.Count > 0 ? 0 : -1;
        }
        void ClearOldRender()
        {
            stack.Children.Clear();
            stackIndicator.Children.Clear();
            gridActiveIndicator.Children.Clear();
        }
        bool VerifyNotRender()
        {
            var willNotlRender = Equals9DigitPrecision(OffsetPercent, 1) ||
            ItemTemplate == null ||
                ItemSource == null;
            return willNotlRender;
        }
        void RenderCarousel()
        {
            foreach (var item in ItemSource)
            {
                var template = ItemTemplate.CreateContent();
                if (!(template is ViewCell cell)) continue;
                cell.View.BindingContext = item;
                cell.View.SetBinding(WidthRequestProperty, new Binding
                {
                    Source = this,
                    Path = nameof(ItemWidth),
                });
                stack.Children.Add(cell.View);
            }
            //add boxview to the last for supporting snaping to right
            if (stack.Children.Count > 0)
            {
                var lastBoxView = new BoxView
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                };
                lastBoxView.SetBinding(WidthRequestProperty, new Binding
                {
                    Source = this,
                    Path = nameof(OffsetWidth),
                });
                stack.Children.Add(lastBoxView);
            }
        }
        void RenderIndicator()
        {

            for (int i = 0; i < stack.Children.Count - 1; i++)
            {
                var dot = new BoxView
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };
                dot.SetBinding(WidthRequestProperty, new Binding
                {
                    Source = this,
                    Path = nameof(IndicatorSize),
                });
                dot.SetBinding(HeightRequestProperty, new Binding
                {
                    Source = this,
                    Path = nameof(IndicatorSize),
                });
                dot.SetBinding(BackgroundColorProperty, new Binding
                {
                    Source = this,
                    Path = nameof(IndicatorColor),
                });
                dot.SetBinding(BoxView.CornerRadiusProperty, new Binding
                {
                    Source = this,
                    Path = nameof(IndicatorRadius),
                });
                var dotContainer = new Grid
                {
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                };
                dotContainer.SetBinding(WidthRequestProperty, new Binding
                {
                    Source = this,
                    Path = nameof(IndicatorWidth),
                });
                dotContainer.Children.Add(dot);
                stackIndicator.Children.Add(dotContainer);
            }
            //add active dot
            if (stackIndicator.Children.Count > 0)
            {
                gridActiveIndicator.Children.Add(_activeIndicatorContainer);
            }
        }
        /// <summary>
        /// Tap handler fire ItemSelected event
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void Handle_Tapped(object sender, System.EventArgs e)
        {
            SelectedObject = ScrollToObject;
            ItemSelected?.Invoke(this, new CarouselViewSelectedItemChangedEventArgs(SelectedIndex, SelectedObject));
        }
    }
}
