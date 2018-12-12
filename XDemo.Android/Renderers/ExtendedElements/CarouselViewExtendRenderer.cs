using Java.Lang;
using System.ComponentModel;
using System.Reflection;
using System.Timers;
using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XDemo.Droid.Renderers.ExtendedElements;
using XDemo.UI.Controls.ExtendedElements;
using Android.Graphics;

[assembly: ExportRenderer(typeof(CarouselViewExtend), typeof(CarouselViewExtendRenderer))]
namespace XDemo.Droid.Renderers.ExtendedElements
{
    public class CarouselViewExtendRenderer : ScrollViewRenderer
    {
        private int _deltaX;
        private Timer _deltaXResetTimer;

        private bool _initialized;
        private bool _motionDown;
        private int _prevScrollX;
        private Timer _scrollStopTimer;
        private HorizontalScrollView _scrollView;


        public CarouselViewExtendRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            //if (e.OldElement != null)
            //{
            //    e.NewElement.PropertyChanged -= ElementPropertyChanged;
            //}

            if (e.NewElement != null)
            {
                _deltaXResetTimer = new Timer(100) { AutoReset = false };
                _deltaXResetTimer.Elapsed += (sender, args) => _deltaX = 0;

                _scrollStopTimer = new Timer(200) { AutoReset = false };
                _scrollStopTimer.Elapsed += (sender, args2) => UpdateSelectedIndex();

                e.NewElement.PropertyChanged += ElementPropertyChanged;
            }
        }

        private void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Renderer")
            {
                _scrollView = (HorizontalScrollView)typeof(ScrollViewRenderer)
                    .GetField("_hScrollView", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(this);

                _scrollView.HorizontalScrollBarEnabled = false;
                _scrollView.Touch += HScrollViewTouch;
            }

            if (e.PropertyName == CarouselViewExtend.SelectedIndexProperty.PropertyName && !_motionDown)
                ScrollToIndex(((CarouselViewExtend)Element).SelectedIndex);
        }

        private void HScrollViewTouch(object sender, TouchEventArgs e)
        {
            e.Handled = false;

            switch (e.Event.Action)
            {
                case MotionEventActions.Move:
                    _deltaXResetTimer.Stop();
                    _deltaX = _scrollView.ScrollX - _prevScrollX;
                    _prevScrollX = _scrollView.ScrollX;

                    UpdateSelectedIndex();

                    _deltaXResetTimer.Start();
                    break;
                case MotionEventActions.Down:
                    _motionDown = true;
                    _scrollStopTimer.Stop();
                    break;
                case MotionEventActions.Up:
                    _motionDown = false;
                    SnapScroll();
                    _scrollStopTimer.Start();
                    break;
            }
        }

        private async void UpdateSelectedIndex()
        {
            var center = _scrollView.ScrollX + _scrollView.Width / 2;
            var carouselView = (CarouselViewExtend)Element;
            await System.Threading.Tasks.Task.Delay(300);
            carouselView.SelectedIndex = center / _scrollView.Width;
        }

        private void SnapScroll()
        {
            var roughIndex = (float)_scrollView.ScrollX / _scrollView.Width;

            var targetIndex =
                _deltaX < 0
                    ? Math.Floor(roughIndex)
                    : _deltaX > 0
                        ? Math.Ceil(roughIndex)
                        : Math.Round(roughIndex);

            ScrollToIndex((int)targetIndex);
        }

        private void ScrollToIndex(int targetIndex)
        {
            var targetX = targetIndex * _scrollView.Width;
            _scrollView.Post(new Runnable(() => { _scrollView.SmoothScrollTo(targetX, 0); }));
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            if (_initialized) return;
            _initialized = true;
            var carouselLayout = (CarouselViewExtend)Element;
            _scrollView.ScrollTo(carouselLayout.SelectedIndex * Width, 0);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            if (_initialized && w != oldw) _initialized = false;
            base.OnSizeChanged(w, h, oldw, oldh);
        }
    }
}
