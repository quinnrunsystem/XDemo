using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XDemo.Core.Infrastructure.Logging;
using XDemo.iOS.Renderers.ExtendedElements;
using XDemo.UI.Controls.ExtendedElements;

[assembly: ExportRenderer(typeof(CarouselViewExtend), typeof(CarouselViewExtendRenderer))]
namespace XDemo.iOS.Renderers.ExtendedElements
{
    public class CarouselViewExtendRenderer : ScrollViewRenderer
    {
        UIScrollView _native;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                _native.Scrolled -= NativeScrolled;
                e.NewElement.PropertyChanged -= ElementPropertyChanged;
            }

            if (e.NewElement != null)
            {
                _native = (UIScrollView)NativeView;
                _native.PagingEnabled = true;
                _native.Bounces = false;
                _native.ShowsHorizontalScrollIndicator = false;
                //_native.Scrolled += NativeScrolled;
                _native.DraggingEnded += _native_DraggingEnded;
                e.NewElement.PropertyChanged += ElementPropertyChanged;
            }
        }

        nfloat lastOffsetX;
        void _native_DraggingEnded(object sender, DraggingEventArgs e)
        {
            LogCommon.Info("_native_DraggingEnded: ContentOffsetX: " + _native.ContentOffset.X);

            //var center = _native.ContentOffset.X + (_native.Bounds.Width / 2);
            //int a = ((int)center) / ((int)_native.Bounds.Width);

            if (lastOffsetX > _native.ContentOffset.X)
            {
                LogCommon.Info("right..");
                if (((CarouselViewExtend)Element).SelectedIndex > 0)
                {
                    ((CarouselViewExtend)Element).SelectedIndex -= 1;

                }

            }
            else
            {
                LogCommon.Info("left..");
                if (((CarouselViewExtend)Element).SelectedIndex < ((CarouselViewExtend)Element).ChildrenList.Count)
                {
                    ((CarouselViewExtend)Element).SelectedIndex += 1;

                }
            }
            _scrl = false;
            lastOffsetX = _native.ContentOffset.X;
        }


        void NativeScrolled(object sender, EventArgs e)
        {
            LogCommon.Info("NativeScrolled");

        }

        void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CarouselViewExtend.SelectedIndexProperty.PropertyName)
            {
                ScrollToSelection(false);
            }
        }

        bool _scrl = false;
        void ScrollToSelection(bool animate)
        {
            if (_scrl)
                return;

            if (Element == null) return;

            LogCommon.Info("ScrollToSelection");
            _native.SetContentOffset(new CoreGraphics.CGPoint
                (_native.Bounds.Width *
                    Math.Max(0, ((CarouselViewExtend)Element).SelectedIndex),
                    _native.ContentOffset.Y),
                animate);
            _scrl = true;
        }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);
            ScrollToSelection(false);
        }
    }
}
