using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Content;
using Android.Support.V4.Widget;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AWidget = Android.Widget;
using AView = Android.Views.View;
using AViewGroup = Android.Views.ViewGroup;
using System.Threading.Tasks;
using Android.Animation;
using XDemo.Droid.Renderers.ExtendedElements.CarouselScrollViews;
using XDemo.UI.Controls.GroupedElements.CarouselScrollViews;

[assembly: ExportRenderer(typeof(CarouselScrollView), typeof(CarouselScrollViewRenderer))]
namespace XDemo.Droid.Renderers.ExtendedElements.CarouselScrollViews
{
    public class CarouselScrollViewRenderer : NestedScrollView, IVisualElementRenderer
    {
        CarouselScrollView _view;
        CustomScrollViewContainer _container;
        AWidget.HorizontalScrollView _hScrollView;
        ScrollBarVisibility _defaultHorizontalScrollVisibility = 0;
        ScrollBarVisibility _defaultVerticalScrollVisibility = 0;
        internal bool ShouldSkipOnTouch;
        bool _isBidirectional;
        LayoutDirection _prevLayoutDirection = LayoutDirection.Ltr;
        bool _isEnabled;
        bool _disposed;
        bool _isAttached;
        int _previousBottom;
        bool _checkedForRtlScroll = false;

        public CarouselScrollViewRenderer(Context context) : base(context)
        {
        }

        public VisualElement Element => _view;

        public VisualElementTracker Tracker { get; private set; }
        protected IScrollViewController Controller => (IScrollViewController)Element;

        public ViewGroup ViewGroup => this;

        public Android.Views.View View => this;

        internal float LastX { get; set; }

        internal float LastY { get; set; }

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
        public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged
        {
            add { ElementPropertyChanged += value; }
            remove { ElementPropertyChanged -= value; }
        }

        public SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            Measure(widthConstraint, heightConstraint);
            return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), new Size(40, 40));
        }

        public void SetElement(VisualElement element)
        {
            CarouselScrollView oldElement = _view;
            _view = (CarouselScrollView)element;

            if (oldElement != null)
            {
                ((IScrollViewController)oldElement).ScrollToRequested -= OnScrollToRequested;
            }
            if (element != null)
            {
                ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(oldElement, element));

                if (_container == null)
                {
                    Tracker = new VisualElementTracker(this);
                    _container = new CustomScrollViewContainer(_view, Context);
                }

                Controller.ScrollToRequested += OnScrollToRequested;

                LoadContent();
                UpdateBackgroundColor();
                UpdateOrientation();
                UpdateIsEnabled();
                UpdateHorizontalScrollBarVisibility();
                UpdateVerticalScrollBarVisibility();
                UpdateFlowDirection();

                if (!string.IsNullOrEmpty(element.AutomationId))
                    ContentDescription = element.AutomationId;
            }
        }

        async void OnScrollToRequested(object sender, ScrollToRequestedEventArgs e)
        {
            _checkedForRtlScroll = true;

            if (!_isAttached)
            {
                return;
            }

            // 99.99% of the time simply queuing to the end of the execution queue should handle this case.
            // However it is possible to end a layout cycle and STILL be layout requested. We want to
            // back off until all are done, even if they trigger layout storms over and over. So we back off
            // for 10ms tops then move on.
            var cycle = 0;
            while (IsLayoutRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                cycle++;

                if (cycle >= 10)
                    break;
            }

            var context = Context;
            var x = (int)context.ToPixels(e.ScrollX);
            var y = (int)context.ToPixels(e.ScrollY);
            int currentX = _view.Orientation == ScrollOrientation.Horizontal || _view.Orientation == ScrollOrientation.Both ? _hScrollView.ScrollX : ScrollX;
            int currentY = _view.Orientation == ScrollOrientation.Vertical || _view.Orientation == ScrollOrientation.Both ? ScrollY : _hScrollView.ScrollY;
            if (e.Mode == ScrollToMode.Element)
            {
                Point itemPosition = Controller.GetScrollPositionForElement(e.Element as VisualElement, e.Position);

                x = (int)context.ToPixels(itemPosition.X);
                y = (int)context.ToPixels(itemPosition.Y);
            }
            if (e.ShouldAnimate)
            {
                ValueAnimator animator = ValueAnimator.OfFloat(0f, 1f);
                animator.SetDuration(100);
                animator.Update += (o, animatorUpdateEventArgs) =>
                {
                    var v = (double)animatorUpdateEventArgs.Animation.AnimatedValue;
                    int distX = GetDistance(currentX, x, v);
                    int distY = GetDistance(currentY, y, v);

                    if (_view == null)
                    {
                        // This is probably happening because the page with this Scroll View
                        // was popped off the stack during animation
                        animator.Cancel();
                        return;
                    }

                    switch (_view.Orientation)
                    {
                        case ScrollOrientation.Horizontal:
                            _hScrollView.ScrollTo(distX, distY);
                            break;
                        case ScrollOrientation.Vertical:
                            ScrollTo(distX, distY);
                            break;
                        default:
                            _hScrollView.ScrollTo(distX, distY);
                            ScrollTo(distX, distY);
                            break;
                    }
                };
                animator.AnimationEnd += delegate
                {
                    if (Controller == null)
                        return;
                    Controller.SendScrollFinished();
                };

                animator.Start();
            }
            else
            {
                switch (_view.Orientation)
                {
                    case ScrollOrientation.Horizontal:
                        _hScrollView.ScrollTo(x, y);
                        break;
                    case ScrollOrientation.Vertical:
                        ScrollTo(x, y);
                        break;
                    default:
                        _hScrollView.ScrollTo(x, y);
                        ScrollTo(x, y);
                        break;
                }
                Controller.SendScrollFinished();
            }
        }
        static int GetDistance(double start, double position, double v)
        {
            return (int)(start + (position - start) * v);
        }

        void LoadContent()
        {
            _container.ChildView = _view.Content;
        }
        public void SetLabelFor(int? id)
        {
        }

        public void UpdateLayout()
        {
            Tracker?.UpdateLayout();
        }
        void UpdateBackgroundColor()
        {
            SetBackgroundColor(Element.BackgroundColor.ToAndroid(Color.Transparent));
        }

        void UpdateOrientation()
        {
            if (_view.Orientation == ScrollOrientation.Horizontal || _view.Orientation == ScrollOrientation.Both)
            {
                if (_hScrollView == null)
                {
                    _hScrollView = new CustomAndroidHorizontalScrollView(Context, this);
                    UpdateFlowDirection();
                }

                ((CustomAndroidHorizontalScrollView)_hScrollView).IsBidirectional = _isBidirectional = _view.Orientation == ScrollOrientation.Both;

                if (_hScrollView.Parent != this)
                {
                    _container.RemoveFromParent();
                    _hScrollView.AddView(_container);
                    AddView(_hScrollView);
                }
            }
            else
            {
                if (_container.Parent != this)
                {
                    _container.RemoveFromParent();
                    if (_hScrollView != null)
                        _hScrollView.RemoveFromParent();
                    AddView(_container);
                }
            }
        }

        void UpdateHorizontalScrollBarVisibility()
        {
            if (_hScrollView != null)
            {
                if (_defaultHorizontalScrollVisibility == 0)
                {
                    _defaultHorizontalScrollVisibility = _hScrollView.HorizontalScrollBarEnabled ? ScrollBarVisibility.Always : ScrollBarVisibility.Never;
                }

                var newHorizontalScrollVisiblility = _view.HorizontalScrollBarVisibility;

                if (newHorizontalScrollVisiblility == ScrollBarVisibility.Default)
                {
                    newHorizontalScrollVisiblility = _defaultHorizontalScrollVisibility;
                }

                _hScrollView.HorizontalScrollBarEnabled = newHorizontalScrollVisiblility == ScrollBarVisibility.Always;
            }
        }

        void UpdateVerticalScrollBarVisibility()
        {
            if (_defaultVerticalScrollVisibility == 0)
                _defaultVerticalScrollVisibility = VerticalScrollBarEnabled ? ScrollBarVisibility.Always : ScrollBarVisibility.Never;

            var newVerticalScrollVisibility = _view.VerticalScrollBarVisibility;

            if (newVerticalScrollVisibility == ScrollBarVisibility.Default)
                newVerticalScrollVisibility = _defaultVerticalScrollVisibility;

            VerticalScrollBarEnabled = newVerticalScrollVisibility == ScrollBarVisibility.Always;
        }
        internal void UpdateScrollPosition(double x, double y)
        {
            if (_view != null)
            {
                if (_view.Orientation == ScrollOrientation.Both)
                {
                    var context = Context;

                    if (x == 0)
                        x = context.FromPixels(_hScrollView.ScrollX);

                    if (y == 0)
                        y = context.FromPixels(ScrollY);
                }

                Controller.SetScrolledPosition(x, y);
            }
        }
        void UpdateFlowDirection()
        {
            if (Element is IVisualElementController controller)
            {
                var flowDirection = controller.EffectiveFlowDirection.IsLeftToRight()
                    ? LayoutDirection.Ltr
                    : LayoutDirection.Rtl;

                if (_prevLayoutDirection != flowDirection && _hScrollView != null)
                {
                    _prevLayoutDirection = flowDirection;
                    _hScrollView.LayoutDirection = flowDirection;
                }
            }
        }
        void UpdateIsEnabled()
        {
            if (Element == null)
            {
                return;
            }

            _isEnabled = Element.IsEnabled;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (Element.InputTransparent)
                return false;

            // set the start point for the bidirectional scroll; 
            // Down is swallowed by other controls, so we'll just sneak this in here without actually preventing
            // other controls from getting the event.           
            if (_isBidirectional && ev.Action == MotionEventActions.Down)
            {
                LastY = ev.RawY;
                LastX = ev.RawX;
            }

            return base.OnInterceptTouchEvent(ev);
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            if (!_isEnabled)
                return false;

            if (ShouldSkipOnTouch)
            {
                ShouldSkipOnTouch = false;
                return false;
            }

            // The nested ScrollViews will allow us to scroll EITHER vertically OR horizontally in a single gesture.
            // This will allow us to also scroll diagonally.
            // We'll fall through to the base event so we still get the fling from the ScrollViews.
            // We have to do this in both ScrollViews, since a single gesture will be owned by one or the other, depending
            // on the initial direction of movement (i.e., horizontal/vertical).
            if (_isBidirectional && !Element.InputTransparent)
            {
                float dX = LastX - ev.RawX;

                LastY = ev.RawY;
                LastX = ev.RawX;
                if (ev.Action == MotionEventActions.Move)
                {
                    foreach (CustomAndroidHorizontalScrollView child in GetChildrenOfType<CustomAndroidHorizontalScrollView>(this))
                    {
                        child.ScrollBy((int)dX, 0);
                        break;
                    }
                    // Fall through to base.OnTouchEvent, it'll take care of the Y scrolling                
                }
            }

            return base.OnTouchEvent(ev);
        }
        static IEnumerable<T> GetChildrenOfType<T>(AViewGroup self) where T : AView
        {
            for (var i = 0; i < self.ChildCount; i++)
            {
                AView child = self.GetChildAt(i);
                var typedChild = child as T;
                if (typedChild != null)
                    yield return typedChild;

                if (child is AViewGroup)
                {
                    IEnumerable<T> myChildren = GetChildrenOfType<T>(child as AViewGroup);
                    foreach (T nextChild in myChildren)
                        yield return nextChild;
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing)
            {
                SetElement(null);
                Tracker?.Dispose();
                Tracker = null;
                RemoveAllViews();
                _container?.Dispose();
                _container = null;
            }

            base.Dispose(disposing);
        }
        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            _isAttached = true;
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();

            _isAttached = false;
        }

        protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
        {
            ElementChanged?.Invoke(this, e);
        }
        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            // If the scroll view has changed size because of soft keyboard dismissal
            // (while WindowSoftInputModeAdjust is set to Resize), then we may need to request a 
            // layout of the ScrollViewContainer
            bool requestContainerLayout = bottom > _previousBottom;
            _previousBottom = bottom;

            _container.Measure(MeasureSpecFactory.MakeMeasureSpec(right - left, MeasureSpecMode.Unspecified),
                MeasureSpecFactory.MakeMeasureSpec(bottom - top, MeasureSpecMode.Unspecified));
            base.OnLayout(changed, left, top, right, bottom);
            if (_view.Content != null && _hScrollView != null)
                _hScrollView.Layout(0, 0, right - left, Math.Max(bottom - top, (int)Context.ToPixels(_view.Content.Height)));
            else if (_view.Content != null && requestContainerLayout)
                _container?.RequestLayout();

            // if the target sdk >= 17 then setting the LayoutDirection on the scroll view natively takes care of the scroll
            if (!_checkedForRtlScroll && _hScrollView != null && Element is IVisualElementController controller && controller.EffectiveFlowDirection.IsRightToLeft())
            {
                if (Context.TargetSdkVersion() < 17)
                    _hScrollView.ScrollX = _container.MeasuredWidth - _hScrollView.MeasuredWidth - _hScrollView.ScrollX;
                else
                    Device.BeginInvokeOnMainThread(() => UpdateScrollPosition(_hScrollView.ScrollX, ScrollY));
            }

            _checkedForRtlScroll = true;
        }

        internal static class MeasureSpecFactory
        {
            public static int GetSize(int measureSpec)
            {
                const int modeMask = 0x3 << 30;
                return measureSpec & ~modeMask;
            }

            // Literally does the same thing as the android code, 1000x faster because no bridge cross
            // benchmarked by calling 1,000,000 times in a loop on actual device
            public static int MakeMeasureSpec(int size, MeasureSpecMode mode)
            {
                return size + (int)mode;
            }
        }

        protected override void OnScrollChanged(int l, int t, int oldl, int oldt)
        {
            _checkedForRtlScroll = true;
            base.OnScrollChanged(l, t, oldl, oldt);
            var context = Context;
            UpdateScrollPosition(context.FromPixels(l), context.FromPixels(t));
        }

    }
}
