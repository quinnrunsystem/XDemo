using System;
using Android.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace XDemo.Droid.Renderers.ExtendedElements.CarouselScrollViews
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
}
