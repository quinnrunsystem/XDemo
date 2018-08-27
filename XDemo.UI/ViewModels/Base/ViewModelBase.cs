using Xamarin.Forms;
using Prism.Navigation;
using PropertyChanged;
using Prism.AppModel;
using System.Threading.Tasks;
using XDemo.UI.Views.Base;
using System.Threading;
using XDemo.UI.Extensions;

namespace XDemo.UI.ViewModels.Base
{
    [AddINotifyPropertyChangedInterface]
    public abstract class ViewModelBase : BindableObject, INavigationAware, IPageLifecycleAware, IApplicationLifecycleAware, IDestructible
    {
        /// <summary>
        /// Occur when this view model instance is destroying, override this method and put all your cleanup code here
        /// </summary>
        public virtual void Destroy() { }
        /// <summary>
        /// Page is appearing, override this thi handle your logics in viewmodel. <para/>
        /// restrict to handle viewmodel's logic from code behind
        /// </summary>
        public virtual void OnAppearing() { }

        /// <summary>
        /// Page is disappearing, override this thi handle your logics in viewmodel. <para/>
        /// restrict to handle viewmodel's logic from code behind
        /// </summary>
        public virtual void OnDisappearing() { }

        /// <summary>
        /// Occur when we navigated to another view from this view
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        public virtual void OnNavigatedFrom(NavigationParameters parameters) { }

        /// <summary>
        /// Occur when we naviaged to this view from others <para/>
        /// More info in <see cref="NavigationMode"/> of the <paramref name="parameters"/>
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
            NavigationExtension.ResetGone();
        }

        /// <summary>
        /// We can use this as the Init method
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        public virtual void OnNavigatingTo(NavigationParameters parameters) { }

        /// <summary>
        /// On the app resume.
        /// </summary>
        public virtual void OnResume() { }

        /// <summary>
        /// On the app sleep
        /// </summary>
        public virtual void OnSleep() { }

        /// <summary>
        /// occur whe user rotated the device
        /// </summary>
        /// <param name="orientation">Orientation.</param>
        public virtual void OnScreenRotated(ScreenOrientation orientation)
        {
        }

        #region base properties

        /// <summary>Identifies the <see cref="P:Xamarin.Forms.Page.Title" /> property.</summary>
        /// <remarks>To be added.</remarks>
        public string Title { get; set; } = "Page title";

        /// <summary>Marks the Page as busy. This will cause the platform specific global activity indicator to show a busy state.</summary>
        /// <value>A bool indicating if the Page is busy or not.</value>
        /// <remarks>Setting IsBusy to true on multiple pages at once will cause the global activity indicator to run until both are set back to false. It is the authors job to unset the IsBusy flag before cleaning up a Page.</remarks>
        public bool IsBusy { get; set; }
        #endregion

       
    }
}
