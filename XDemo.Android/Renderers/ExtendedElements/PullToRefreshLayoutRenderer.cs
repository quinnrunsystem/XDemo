/*
 * Copyright (C) 2015 Refractored LLC & James Montemagno: 
 * http://github.com/JamesMontemagno
 * http://twitter.com/JamesMontemagno
 * http://refractored.com
 * 
 * The MIT License (MIT) see GitHub For more information
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.ComponentModel;
using System.Reflection;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XDemo.Droid.Renderers.ExtendedElements;
using XDemo.UI.Controls.ExtendedElements;
using Android.Content;
using XDemo.Core.Infrastructure.Logging;

[assembly: ExportRenderer(typeof(PullToRefreshLayout), typeof(PullToRefreshLayoutRenderer))]
namespace XDemo.Droid.Renderers.ExtendedElements
{
    /// <summary>
    /// Pull to refresh layout renderer.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class PullToRefreshLayoutRenderer : SwipeRefreshLayout, IVisualElementRenderer, SwipeRefreshLayout.IOnRefreshListener
    {
        private readonly Context _context;
        public PullToRefreshLayoutRenderer(Context context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Occurs when element changed.
        /// </summary>
        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
        public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

        bool _init;
        IVisualElementRenderer _packed;
        /// <summary>
        /// Setup our SwipeRefreshLayout and register for property changed notifications.
        /// </summary>
        /// <param name="element">Element.</param>
        public void SetElement(VisualElement element)
        {
            var oldElement = Element;

            //unregister old and re-register new
            if (oldElement != null)
                oldElement.PropertyChanged -= HandleElementPropertyChanged;

            Element = element;
            if (Element != null)
            {
                UpdateContent();
                Element.PropertyChanged += HandleElementPropertyChanged;
            }

            if (!_init)
            {
                _init = true;
                //sizes to match the forms view
                //updates properties, handles visual element properties
                Tracker = new VisualElementTracker(this);
                SetOnRefreshListener(this);
            }

            UpdateColors();
            UpdateIsRefreshing();
            UpdateIsSwipeToRefreshEnabled();

            ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(oldElement, this.Element));
        }

        /// <summary>
        /// Managest adding and removing the android viewgroup to our actual swiperefreshlayout
        /// </summary>
        void UpdateContent()
        {
            if (RefreshView.Content == null)
                return;

            if (_packed != null)
                RemoveView(_packed.View);

            _packed = Platform.CreateRendererWithContext(RefreshView.Content, _context);

            try
            {
                RefreshView.Content.SetValue(RendererProperty, _packed);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to sent renderer property, maybe an issue: " + ex);
            }

            AddView(_packed.View, LayoutParams.MatchParent);
        }

        BindableProperty _rendererProperty = null;

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

                var type = Type.GetType("Xamarin.Forms.Platform.Android.Platform, Xamarin.Forms.Platform.Android");
                var prop = type.GetField("RendererProperty", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                var val = prop.GetValue(null);
                _rendererProperty = val as BindableProperty;

                return _rendererProperty;
            }
        }

        void UpdateColors()
        {
            if (RefreshView == null)
                return;
            if (RefreshView.RefreshColor != Color.Default)
                SetColorSchemeColors(RefreshView.RefreshColor.ToAndroid());
            if (RefreshView.RefreshBackgroundColor != Color.Default)
                SetProgressBackgroundColorSchemeColor(RefreshView.RefreshBackgroundColor.ToAndroid());
        }

        bool _refreshing;
        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="T:XDemo.Droid.Renderers.ExtendedElements.PullToRefreshLayoutRenderer"/> is refreshing.
        /// </summary>
        /// <value><c>true</c> if refreshing; otherwise, <c>false</c>.</value>
        public override bool Refreshing
        {
            get
            {
                return _refreshing;
            }
            set
            {
                try
                {
                    _refreshing = value;
                    //this will break binding :( sad panda we need to wait for next version for this
                    //right now you can't update the binding.. so it is 1 way
                    if (RefreshView != null && RefreshView.IsRefreshing != _refreshing)
                        RefreshView.IsRefreshing = _refreshing;

                    if (base.Refreshing == _refreshing)
                        return;

                    base.Refreshing = _refreshing;
                }
                catch (Exception ex)
                {
                    LogCommon.Info("Unable to change IsRefreshing: " + ex);
                }
            }
        }

        void UpdateIsRefreshing() =>
            Refreshing = RefreshView.IsRefreshing;


        void UpdateIsSwipeToRefreshEnabled() =>
            Enabled = RefreshView.IsPullToRefreshEnabled;



        /// <summary>
        /// Determines whether this instance can child scroll up.
        /// We do this since the actual swipe refresh can't figure it out
        /// </summary>
        /// <returns><c>true</c> if this instance can child scroll up; otherwise, <c>false</c>.</returns>
        public override bool CanChildScrollUp() =>
            CanScrollUp(_packed.View);


        bool CanScrollUp(Android.Views.View view)
        {
            var viewGroup = view as ViewGroup;
            if (viewGroup == null)
                return base.CanChildScrollUp();

            var sdk = Android.OS.Build.VERSION.SdkInt;
            if (sdk >= Android.OS.BuildVersionCodes.JellyBean)
            {
                //is a scroll container such as listview, scroll view, gridview
                if (viewGroup.IsScrollContainer)
                {
                    return base.CanChildScrollUp();
                }
            }

            //if you have something custom and you can't scroll up you might need to enable this
            //for instance on a custom recycler view where the code above isn't working!
            for (int i = 0; i < viewGroup.ChildCount; i++)
            {
                var child = viewGroup.GetChildAt(i);
                if (child is Android.Widget.AbsListView)
                {
                    var list = child as Android.Widget.AbsListView;
                    if (list != null)
                    {
                        if (list.FirstVisiblePosition == 0)
                        {
                            var subChild = list.GetChildAt(0);

                            return subChild != null && subChild.Top != 0;
                        }

                        //if children are in list and we are scrolled a bit... sure you can scroll up
                        return true;
                    }

                }
                else if (child is Android.Widget.ScrollView)
                {
                    var scrollview = child as Android.Widget.ScrollView;
                    return (scrollview.ScrollY <= 0.0);
                }
                else if (child is Android.Webkit.WebView)
                {
                    var webView = child as Android.Webkit.WebView;
                    return (webView.ScrollY > 0.0);
                }
                else if (child is Android.Support.V4.Widget.SwipeRefreshLayout)
                {
                    return CanScrollUp(child as ViewGroup);
                }
                //else if something else like a recycler view?

            }

            return false;
        }


        /// <summary>
        /// Helpers to cast our element easily
        /// Will throw an exception if the Element is not correct
        /// </summary>
        /// <value>The refresh view.</value>
        public PullToRefreshLayout RefreshView =>
            Element == null ? null : (PullToRefreshLayout)Element;

        /// <summary>
        /// The refresh view has been refreshed
        /// </summary>
        public void OnRefresh()
        {
            if (RefreshView?.RefreshCommand?.CanExecute(RefreshView?.RefreshCommandParameter) ?? false)
            {
                RefreshView.RefreshCommand.Execute(RefreshView?.RefreshCommandParameter);
            }
        }


        /// <summary>
        /// Handles the property changed.
        /// Update the control and trigger refreshing
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void HandleElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PullToRefreshLayout.Content):
                    UpdateContent();
                    break;
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
        /// Gets the size of the desired.
        /// </summary>
        /// <returns>The desired size.</returns>
        /// <param name="widthConstraint">Width constraint.</param>
        /// <param name="heightConstraint">Height constraint.</param>
        public SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            _packed.View.Measure(widthConstraint, heightConstraint);

            //Measure child here and determine size
            return new SizeRequest(new Size(_packed.View.MeasuredWidth, _packed.View.MeasuredHeight));
        }

        /// <summary>
        /// Updates the layout.
        /// </summary>
        public void UpdateLayout() => Tracker?.UpdateLayout();


        /// <summary>
        /// Gets the tracker.
        /// </summary>
        /// <value>The tracker.</value>
        public VisualElementTracker Tracker { get; private set; }


        /// <summary>
        /// Gets the view group.
        /// </summary>
        /// <value>The view group.</value>
        public Android.Views.ViewGroup ViewGroup => this;


        public Android.Views.View View => this;

        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value>The element.</value>
        public VisualElement Element { get; private set; }

        /// <summary>
        /// Cleanup layout.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Element != null)
                {
                    Element.PropertyChanged -= HandleElementPropertyChanged;
                }

                if (_packed != null)
                    RemoveView(_packed.View);
            }

            _packed?.Dispose();
            _packed = null;

            Tracker?.Dispose();
            Tracker = null;
            _rendererProperty = null;
            _init = false;
        }

        public void SetLabelFor(int? id)
        {

        }
    }
}

