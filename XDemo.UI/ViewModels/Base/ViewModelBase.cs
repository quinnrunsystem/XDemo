using Xamarin.Forms;
using Prism.Navigation;
using PropertyChanged;
using Prism.AppModel;
using XDemo.UI.Views.Base;
using System.Threading.Tasks;
using System.Threading;
using XDemo.UI.Utils;

namespace XDemo.UI.ViewModels.Base
{
    /* ==================================================================================================
     * use this attribute for simplier property define in all our viewmodels
     * ================================================================================================*/
    [AddINotifyPropertyChangedInterface]
    public abstract class ViewModelBase : BindableObject, INavigationAware, IPageLifecycleAware, IApplicationLifecycleAware, IDestructible
    {
        private readonly INavigationService _navigationService;
        protected ViewModelBase(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
        /// <summary>
        /// Occur when this view model instance is destroying, override this method and put all your cleanup code here
        /// </summary>
        public virtual void Destroy()
        {
        }

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
        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        /// <summary>
        /// Occur when we naviaged to this view from others <para/>
        /// More info in <see cref="NavigationMode"/> of the <paramref name="parameters"/>
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        /// <summary>
        /// We can use this as the Init method
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        public virtual void OnNavigatingTo(INavigationParameters parameters) { }

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
        /// <param name="orientation">Current orientation of device.</param>
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

        #region navigate methods

        /// <summary>
        /// The semaphore. <para/>
        /// In case user press a back button in our app and the back button on device (android) or swipeback gesture (iOS) at same time. <para/>
        /// </summary>
        #region Push async
        /// <summary>
        /// navigate to a view model async
        /// </summary>
        /// <returns>The to async.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        /// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
        protected Task PushAsync<TViewModel>(INavigationParameters parameters, bool animated = true) where TViewModel : ViewModelBase
        {
            return PushAsync(typeof(TViewModel).Name, parameters, animated);
        }

        /// <summary>
        /// navigate to a viewmodel async without navigation parameters
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        /// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
        protected Task PushAsync<TViewModel>(bool animated = true) where TViewModel : ViewModelBase
        {
            return PushAsync<TViewModel>(null, animated);
        }

        private readonly object _monitor = new object();
        private readonly int _delyTimeInMs = 1000;
        /// <summary>
        /// navigate to a view model async
        /// </summary>
        /// <returns>The to async.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        /// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
        protected async Task PushAsync(string uri, INavigationParameters parameters = null, bool animated = true)
        {
            /* ==================================================================================================
             * async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
             * ================================================================================================*/
            if (Monitor.IsEntered(_monitor))
                return;
            Monitor.Enter(_monitor);
            var navigateRs = await _navigationService.NavigateAsync(uri, parameters, animated: animated);
            /* ==================================================================================================
             * temporary use. b/c although the method returned a result. but the UI maybe not changed
             * ================================================================================================*/
            await Task.Delay(_delyTimeInMs);
            Monitor.Exit(_monitor);
        }

        #endregion

        #region Push modal async
        protected Task PushModalAsync<TViewModel>(bool animated = true) where TViewModel : ViewModelBase
        {
            return PushModalAsync(typeof(TViewModel).Name, null, animated);
        }

        protected Task PushModalAsync<TViewModel>(INavigationParameters parameters, bool animated = true) where TViewModel : ViewModelBase
        {
            return PushModalAsync(typeof(TViewModel).Name, parameters, animated);
        }

        protected async Task PushModalAsync(string uri, INavigationParameters parameters = null, bool animated = true)
        {
            /* ==================================================================================================
             * async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
             * ================================================================================================*/
            if (Monitor.IsEntered(_monitor))
                return;
            Monitor.Enter(_monitor);
            await _navigationService.NavigateAsync(uri, parameters, true, animated);
            /* ==================================================================================================
             * temporary use. b/c although the method returned a result. but the UI maybe not changed
             * ================================================================================================*/
            await Task.Delay(_delyTimeInMs);
            Monitor.Exit(_monitor);
        }
        #endregion
        protected Task PopAsync(bool animated = true)
        {
            return PopAsync(null, animated);
        }

        /// <summary>
        /// let's back
        /// </summary>
        /// <returns>The back async.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        protected async Task PopAsync(INavigationParameters parameters, bool animated = true)
        {
            /* ==================================================================================================
             * async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
             * ================================================================================================*/
            if (Monitor.IsEntered(_monitor))
                return;
            Monitor.Enter(_monitor);
            var rs = await _navigationService.GoBackAsync(parameters, null, animated);
            /* ==================================================================================================
             * temporary use. b/c although the method returned a result. but the UI maybe not changed
             * ================================================================================================*/
            await Task.Delay(_delyTimeInMs);
            Monitor.Exit(_monitor);
        }

        protected Task PopModalAsync(bool animated = true)
        {
            return PopModalAsync(null, animated);
        }

        protected async Task PopModalAsync(INavigationParameters parameters, bool animated = true)
        {
            /* ==================================================================================================
             * async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
             * ================================================================================================*/
            if (Monitor.IsEntered(_monitor))
                return;
            Monitor.Enter(_monitor);
            var rs = await _navigationService.GoBackAsync(parameters, true, animated);
            /* ==================================================================================================
               * temporary use. b/c although the method returned a result. but the UI maybe not changed
               * ================================================================================================*/
            await Task.Delay(_delyTimeInMs);
            Monitor.Exit(_monitor);
        }

        /// <summary>
        /// let's back to root
        /// </summary>
        /// <returns>The back to root async.</returns>
        /// <param name="parameters">Parameters.</param>
        protected async Task PopToRootAsync(INavigationParameters parameters = null)
        {
            /* ==================================================================================================
             * async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
             * ================================================================================================*/
            if (Monitor.IsEntered(_monitor))
                return;
            Monitor.Enter(_monitor);
            await _navigationService.GoBackToRootAsync(parameters);
            /* ==================================================================================================
             * temporary use. b/c although the method returned a result. but the UI maybe not changed
             * ================================================================================================*/
            await Task.Delay(_delyTimeInMs);
            Monitor.Exit(_monitor);
        }

        /// <summary>
        /// where the place on earth i'm in?
        /// </summary>
        /// <returns>The am i.</returns>
        protected string WhereAmI()
        {
            return _navigationService.GetNavigationUriPath();
        }
        #endregion

        #region pre-defined
        /// <summary>
        /// Goes to main page async. <para/>
        /// 
        /// </summary>
        /// <returns>The to main page async.</returns>
        protected async Task GoToMainPageAsync()
        {
            /* ==================================================================================================
             * async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
             * ================================================================================================*/
            if (Monitor.IsEntered(_monitor))
                return;
            Monitor.Enter(_monitor);
            await NavigationHelper.GoToMainPageAsync(_navigationService);
            /* ==================================================================================================
             * temporary use. b/c although the method returned a result. but the UI maybe not changed
             * ================================================================================================*/
            await Task.Delay(_delyTimeInMs);
            Monitor.Exit(_monitor);
        }

        protected async Task GoToLoginPageAsync()
        {
            /* ==================================================================================================
             * async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
             * ================================================================================================*/
            if (Monitor.IsEntered(_monitor))
                return;
            Monitor.Enter(_monitor);
            await NavigationHelper.GoToLoginPageAsync(_navigationService);
            /* ==================================================================================================
             * temporary use. b/c although the method returned a result. but the UI maybe not changed
             * ================================================================================================*/
            await Task.Delay(_delyTimeInMs);
            Monitor.Exit(_monitor);
        }
        #endregion
    }
}
