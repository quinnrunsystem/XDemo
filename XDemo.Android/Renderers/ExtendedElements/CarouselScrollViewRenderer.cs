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
using XDemo.UI.Controls.ExtendedElements;
using XDemo.Droid.Renderers.ExtendedElements;

[assembly: ExportRenderer(typeof(HorizontalScrollView), typeof(CarouselScrollViewRenderer))]
namespace XDemo.Droid.Renderers.ExtendedElements
{
    internal class CustomScrollViewContainer : ViewGroup
    {
        readonly ScrollView _parent;
        Xamarin.Forms.View _childView;

        public CustomScrollViewContainer(ScrollView parent, Context context) : base(context)
        {
            _parent = parent;
        }

        public Xamarin.Forms.View ChildView
        {
            get { return _childView; }
            set
            {
                if (_childView == value)
                    return;

                RemoveAllViews();

                _childView = value;

                if (_childView == null)
                    return;

                IVisualElementRenderer renderer;
                if ((renderer = Platform.GetRenderer(_childView)) == null)
                    Platform.SetRenderer(_childView, renderer = CreateRenderer(_childView, Context));

                if (renderer.View.Parent != null)
                    renderer.View.RemoveFromParent();

                AddView(renderer.View);
            }
        }
        internal static IVisualElementRenderer CreateRenderer(VisualElement element, Context context)
        {
            IVisualElementRenderer renderer = Xamarin.Forms.Internals.Registrar.Registered.GetHandlerForObject<IVisualElementRenderer>(element, context)
                ?? new DefaultRenderer(context);
            renderer.SetElement(element);

            return renderer;
        }
        internal class DefaultRenderer : VisualElementRenderer<Xamarin.Forms.View>
        {
            public bool NotReallyHandled { get; private set; }
            IOnTouchListener _touchListener;

            [Obsolete("This constructor is obsolete as of version 2.5. Please use DefaultRenderer(Context) instead.")]
            public DefaultRenderer()
            {
            }

            readonly MotionEventHelper _motionEventHelper = new MotionEventHelper();
            internal class MotionEventHelper
            {
                VisualElement _element;
                bool _isInViewCell;

                public bool HandleMotionEvent(IViewParent parent, MotionEvent motionEvent)
                {
                    if (_isInViewCell || motionEvent.Action == MotionEventActions.Cancel)
                    {
                        return false;
                    }

                    var renderer = parent as DefaultRenderer;
                    if (renderer == null || ShouldPassThroughElement())
                    {
                        return false;
                    }

                    // Let the container know that we're "fake" handling this event
                    renderer.NotifyFakeHandling();

                    return true;
                }

                public void UpdateElement(VisualElement element)
                {
                    _isInViewCell = false;
                    _element = element;

                    if (_element == null)
                    {
                        return;
                    }

                    // Determine whether this control is inside a ViewCell;
                    // we don't fake handle the events because ListView needs them for row selection
                    _isInViewCell = IsInViewCell(element);
                }
                public static bool IsInViewCell(VisualElement element)
                {
                    var parent = element.Parent;
                    while (parent != null)
                    {
                        if (parent is ViewCell)
                        {
                            return true;
                        }
                        parent = parent.Parent;
                    }

                    return false;
                }

                bool ShouldPassThroughElement()
                {
                    if (_element is Layout layout)
                    {
                        if (!layout.InputTransparent)
                        {
                            // If the layout is not input transparent, then the event should not pass through it
                            return false;
                        }

                        if (layout.CascadeInputTransparent)
                        {
                            // This is a layout, and it's transparent, and all its children are transparent, then the event
                            // can just pass through 
                            return true;
                        }

                        if (Platform.GetRenderer(_element) is DefaultRenderer renderer)
                        {
                            // If the event is being bubbled up from a child which is not inputtransparent, we do not want
                            // it to be passed through (just up the tree)
                            if (renderer.NotReallyHandled)
                            {
                                return false;
                            }
                        }

                        // This event isn't being bubbled up by a non-InputTransparent child layout
                        return true;
                    }

                    if (_element.InputTransparent)
                    {
                        // This is not a layout and it's transparent; the event can just pass through 
                        return true;
                    }

                    return false;
                }
            }

            public DefaultRenderer(Context context) : base(context)
            {
                ChildrenDrawingOrderEnabled = true;
            }

            internal void NotifyFakeHandling()
            {
                NotReallyHandled = true;
            }

            public override bool OnTouchEvent(MotionEvent e)
            {
                if (base.OnTouchEvent(e))
                    return true;

                return _motionEventHelper.HandleMotionEvent(Parent, e);
            }

            protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
            {
                base.OnElementChanged(e);

                _motionEventHelper.UpdateElement(e.NewElement);
            }

            public override bool DispatchTouchEvent(MotionEvent e)
            {
                #region Excessive explanation
                // Normally dispatchTouchEvent feeds the touch events to its children one at a time, top child first,
                // (and only to the children in the hit-test area of the event) stopping as soon as one of them has handled
                // the event. 

                // But to be consistent across the platforms, we don't want this behavior; if an element is not input transparent
                // we don't want an event to "pass through it" and be handled by an element "behind/under" it. We just want the processing
                // to end after the first non-transparent child, regardless of whether the event has been handled.

                // This is only an issue for a couple of controls; the interactive controls (switch, button, slider, etc) already "handle" their touches 
                // and the events don't propagate to other child controls. But for image, label, and box that doesn't happen. We can't have those controls 
                // lie about their events being handled because then the events won't propagate to *parent* controls (e.g., a frame with a label in it would
                // never get a tap gesture from the label). In other words, we *want* parent propagation, but *do not want* sibling propagation. So we need to short-circuit 
                // base.DispatchTouchEvent here, but still return "false".

                // Duplicating the logic of ViewGroup.dispatchTouchEvent and modifying it slightly for our purposes is a non-starter; the method is too
                // complex and does a lot of micro-optimization. Instead, we provide a signalling mechanism for the controls which don't already "handle" touch
                // events to tell us that they will be lying about handling their event; they then return "true" to short-circuit base.DispatchTouchEvent.

                // The container gets this message and after it gets the "handled" result from dispatchTouchEvent, 
                // it then knows to ignore that result and return false/unhandled. This allows the event to propagate up the tree.
                #endregion

                NotReallyHandled = false;

                var result = base.DispatchTouchEvent(e);

                if (result && NotReallyHandled)
                {
                    // If the child control returned true from its touch event handler but signalled that it was a fake "true", then we
                    // don't consider the event truly "handled" yet. 
                    // Since a child control short-circuited the normal dispatchTouchEvent stuff, this layout never got the chance for
                    // IOnTouchListener.OnTouch and the OnTouchEvent override to try handling the touches; we'll do that now
                    // Any associated Touch Listeners are called from DispatchTouchEvents if all children of this view return false
                    // So here we are simulating both calls that would have typically been called from inside DispatchTouchEvent
                    // but were not called due to the fake "true"
                    result = _touchListener?.OnTouch(this, e) ?? false;
                    return result || OnTouchEvent(e);
                }

                return result;
            }

            public override void SetOnTouchListener(IOnTouchListener l)
            {
                _touchListener = l;
                base.SetOnTouchListener(l);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    _touchListener = null;

                base.Dispose(disposing);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ChildCount > 0)
                    GetChildAt(0).Dispose();
                RemoveAllViews();
                _childView = null;
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            if (_childView == null)
                return;

            IVisualElementRenderer renderer = Platform.GetRenderer(_childView);
            renderer.UpdateLayout();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            // we need to make sure we are big enough to be laid out at 0,0
            if (_childView != null)
            {
                SetMeasuredDimension((int)Context.ToPixels(_childView.Bounds.Right + _parent.Padding.Right), (int)Context.ToPixels(_childView.Bounds.Bottom + _parent.Padding.Bottom));
            }
            else
                SetMeasuredDimension((int)Context.ToPixels(_parent.Padding.Right), (int)Context.ToPixels(_parent.Padding.Bottom));
        }
    }
    public class CustomAHorizontalScrollView : AWidget.HorizontalScrollView
    {
        readonly CarouselScrollViewRenderer _renderer;

        public CustomAHorizontalScrollView(Context context, CarouselScrollViewRenderer renderer) : base(context)
        {
            _renderer = renderer;
        }

        internal bool IsBidirectional { get; set; }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            // set the start point for the bidirectional scroll; 
            // Down is swallowed by other controls, so we'll just sneak this in here without actually preventing
            // other controls from getting the event.
            if (IsBidirectional && ev.Action == MotionEventActions.Down)
            {
                _renderer.LastY = ev.RawY;
                _renderer.LastX = ev.RawX;
            }

            return base.OnInterceptTouchEvent(ev);
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            // If the touch is caught by the horizontal scrollview, forward it to the parent so custom renderers can be notified of the touch.
            var verticalScrollViewerRenderer = Parent as CarouselScrollViewRenderer;
            if (verticalScrollViewerRenderer != null)
            {
                verticalScrollViewerRenderer.ShouldSkipOnTouch = true;
                verticalScrollViewerRenderer.OnTouchEvent(ev);
            }

            // The nested ScrollViews will allow us to scroll EITHER vertically OR horizontally in a single gesture.
            // This will allow us to also scroll diagonally.
            // We'll fall through to the base event so we still get the fling from the ScrollViews.
            // We have to do this in both ScrollViews, since a single gesture will be owned by one or the other, depending
            // on the initial direction of movement (i.e., horizontal/vertical).
            if (IsBidirectional)
            {
                float dY = _renderer.LastY - ev.RawY;

                _renderer.LastY = ev.RawY;
                _renderer.LastX = ev.RawX;
                if (ev.Action == MotionEventActions.Move)
                {
                    var parent = (global::Android.Widget.ScrollView)Parent;
                    parent.ScrollBy(0, (int)dY);
                    // Fall through to base.OnTouchEvent, it'll take care of the X scrolling                    
                }
            }

            //TODO change structure
            #region SnapAction in CarouselView
            if (_renderer != null)
                if (ev.Action == MotionEventActions.Up)
                {
                    OffsetCarouselView GetCarouselView(Element element)
                    {
                        if (element == null) return null;
                        if (element.Parent is OffsetCarouselView carouselView) return carouselView;
                        return GetCarouselView(element.Parent);
                    }
                    //TODO change structure
                    var resultCarouselView = GetCarouselView(_renderer.Element);
                    resultCarouselView?.SnapHandler();
                }
            #endregion

            return base.OnTouchEvent(ev);
        }

        protected override void OnScrollChanged(int l, int t, int oldl, int oldt)
        {
            base.OnScrollChanged(l, t, oldl, oldt);

            _renderer.UpdateScrollPosition(Context.FromPixels(l), Context.FromPixels(t));
        }
        public override void Fling(int velocityX)
        {
            //TODO change structure
            // base.Fling(velocityX); //we dont need this in carousel view
        }
    }
    public class CarouselScrollViewRenderer : NestedScrollView, IVisualElementRenderer
    {
        HorizontalScrollView _view;
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
            HorizontalScrollView oldElement = _view;
            _view = (HorizontalScrollView)element;

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
                    _hScrollView = new CustomAHorizontalScrollView(Context, this);
                    UpdateFlowDirection();
                }

                ((CustomAHorizontalScrollView)_hScrollView).IsBidirectional = _isBidirectional = _view.Orientation == ScrollOrientation.Both;

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
                    foreach (CustomAHorizontalScrollView child in GetChildrenOfType<CustomAHorizontalScrollView>(this))
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
