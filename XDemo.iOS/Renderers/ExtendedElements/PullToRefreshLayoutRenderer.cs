using System;
using System.ComponentModel;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XDemo.iOS.Renderers.ExtendedElements;
using XDemo.UI.Controls.ExtendedElements;
using XDemo.Core.Infrastructure.Logging;

[assembly: ExportRenderer(typeof(PullToRefreshLayout), typeof(PullToRefreshLayoutRenderer))]
namespace XDemo.iOS.Renderers.ExtendedElements
{
    /// <summary>
    /// Pull to refresh layout renderer.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class PullToRefreshLayoutRenderer : ViewRenderer<PullToRefreshLayout, UIView>
    {
        /// <summary>
        /// The main refresh control.
        /// </summary>
        UIRefreshControl _refreshControl;

        /// <summary>
        /// Raises the element changed event.
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<PullToRefreshLayout> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;
            /* ==================================================================================================
             * init the main refresh control
             * ================================================================================================*/
            _refreshControl = new UIRefreshControl();
            _refreshControl.ValueChanged += OnRefresh;

            try
            {
                TryInsertRefresh(this);
            }
            catch (Exception ex)
            {
                LogCommon.Info("View is not supported in PullToRefreshLayout: " + ex);
            }

            UpdateColors();
            UpdateIsRefreshing();
            UpdateIsSwipeToRefreshEnabled();
        }

        bool _set;
        nfloat _origininalY;

        /// <summary>
        /// Make the refresh control work
        /// </summary>
        /// <returns><c>true</c>, if offset refresh was tryed, <c>false</c> otherwise.</returns>
        /// <param name="view">View.</param>
        /// <param name="refreshing">If set to <c>true</c> refreshing.</param>
        /// <param name="index">Index.</param>
        bool TryOffsetRefresh(UIView view, bool refreshing, int index = 0)
        {
            switch (view)
            {
                case UITableView uiTableView:
                {
                    if (!_set)
                    {
                        _origininalY = uiTableView.ContentOffset.Y;
                        _set = true;
                    }

                    if (_origininalY < 0)
                        return true;

                    if (refreshing)
                        uiTableView.SetContentOffset(new CoreGraphics.CGPoint(0, _origininalY - _refreshControl.Frame.Size.Height), true);
                    else
                        uiTableView.SetContentOffset(new CoreGraphics.CGPoint(0, _origininalY), true);
                    return true;
                }
                case UICollectionView uiCollectionView:
                {
                    if (!_set)
                    {
                        _origininalY = uiCollectionView.ContentOffset.Y;
                        _set = true;
                    }

                    if (_origininalY < 0)
                        return true;

                    if (refreshing)
                        uiCollectionView.SetContentOffset(new CoreGraphics.CGPoint(0, _origininalY - _refreshControl.Frame.Size.Height), true);
                    else
                        uiCollectionView.SetContentOffset(new CoreGraphics.CGPoint(0, _origininalY), true);
                    return true;
                }
                case UIWebView _:
                    //can't do anything
                    return true;
                case UIScrollView uiScrollView:
                {
                    if (!_set)
                    {
                        _origininalY = uiScrollView.ContentOffset.Y;
                        _set = true;
                    }

                    if (_origininalY < 0)
                        return true;

                    if (refreshing)
                        uiScrollView.SetContentOffset(new CoreGraphics.CGPoint(0, _origininalY - _refreshControl.Frame.Size.Height), true);
                    else
                        uiScrollView.SetContentOffset(new CoreGraphics.CGPoint(0, _origininalY), true);
                    return true;
                }
            }

            if (view.Subviews == null)
                return false;

            for (var i = 0; i < view.Subviews.Length; i++)
            {
                var control = view.Subviews[i];
                if (TryOffsetRefresh(control, refreshing, i))
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Attach the refresh control to target
        /// </summary>
        /// <returns><c>true</c>, if insert refresh was tryed, <c>false</c> otherwise.</returns>
        /// <param name="view">View.</param>
        /// <param name="index">Index.</param>
        bool TryInsertRefresh(UIView view, int index = 0)
        {
            switch (view)
            {
                case UITableView uiTableView:
                    uiTableView = uiTableView;
                    uiTableView.InsertSubview(_refreshControl, index);
                    return true;
                case UICollectionView uiCollectionView:
                    uiCollectionView = uiCollectionView;
                    uiCollectionView.InsertSubview(_refreshControl, index);
                    return true;
                //todo: review iOS 12 compability
                case UIWebView uiWebView:
                    uiWebView.ScrollView.InsertSubview(_refreshControl, index);
                    return true;
                case UIScrollView uiScrollView:
                    uiScrollView.InsertSubview(_refreshControl, index);
                    uiScrollView.AlwaysBounceVertical = true;
                    return true;
            }

            if (view.Subviews == null)
                return false;

            for (var i = 0; i < view.Subviews.Length; i++)
            {
                var control = view.Subviews[i];
                if (TryInsertRefresh(control, i))
                    return true;
            }

            return false;
        }


        BindableProperty _rendererProperty;

        /// <summary>
        /// Gets the bindable property.
        /// </summary>
        /// <returns>The bindable property.</returns>
        BindableProperty RendererProperty
        {
            get
            {
                if (_rendererProperty != null)
                    return _rendererProperty;

                var type = Type.GetType("Xamarin.Forms.Platform.iOS.Platform, Xamarin.Forms.Platform.iOS");
                var prop = type.GetField("RendererProperty");
                var val = prop.GetValue(null);
                _rendererProperty = val as BindableProperty;

                return _rendererProperty;
            }
        }

        void UpdateColors()
        {
            var refreshView = Element;
            if (refreshView == null)
                return;
            if (refreshView.RefreshColor != Color.Default)
                _refreshControl.TintColor = refreshView.RefreshColor.ToUIColor();
            if (refreshView.RefreshBackgroundColor != Color.Default)
                _refreshControl.BackgroundColor = refreshView.RefreshBackgroundColor.ToUIColor();
        }


        void UpdateIsRefreshing()
        {
            IsRefreshing = Element?.IsRefreshing ?? false;
        }

        void UpdateIsSwipeToRefreshEnabled()
        {
            _refreshControl.Enabled = Element?.IsPullToRefreshEnabled ?? false;
        }

        /* ==================================================================================================
         * Dont use like this, the cast action will be run so many times!
         * public PullToRefreshLayout RefreshView
         * {
         *      get { return Element; }
         * }
         * ================================================================================================*/
    
        bool _isRefreshing;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is refreshing.
        /// </summary>
        /// <value><c>true</c> if this instance is refreshing; otherwise, <c>false</c>.</value>
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                var changed = IsRefreshing != value;

                _isRefreshing = value;
                if (_isRefreshing)
                    _refreshControl.BeginRefreshing();
                else
                    _refreshControl.EndRefreshing();

                if (changed)
                    TryOffsetRefresh(this, IsRefreshing);
            }
        }

        /// <summary>
        /// The refresh view has been refreshed
        /// </summary>
        void OnRefresh(object sender, EventArgs e)
        {
            var refreshView = Element;
            if (refreshView?.RefreshCommand?.CanExecute(refreshView?.RefreshCommandParameter) ?? false)
            {
                refreshView.RefreshCommand.Execute(refreshView?.RefreshCommandParameter);
            }
        }

        /// <summary>
        /// Raises the element property changed event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case nameof(PullToRefreshLayout.IsPullToRefreshEnabled):
                    UpdateIsSwipeToRefreshEnabled();
                    break;
                case nameof(PullToRefreshLayout.IsRefreshing):
                    UpdateIsRefreshing();
                    break;
                case nameof(PullToRefreshLayout.RefreshColor):
                case nameof(PullToRefreshLayout.RefreshBackgroundColor):
                    UpdateColors();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Dispose the specified disposing.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_refreshControl != null)
            {
                _refreshControl.ValueChanged -= OnRefresh;
            }
        }
    }
}

