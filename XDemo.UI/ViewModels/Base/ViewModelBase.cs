using Xamarin.Forms;
using Prism.Navigation;
using PropertyChanged;
using Prism.AppModel;
using XDemo.UI.Views.Base;
using System.Threading.Tasks;
using System;
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
            _semaphore.Dispose();
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
            _wasGone = false;
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
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _wasGone;
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

        /// <summary>
        /// navigate to a view model async
        /// </summary>
        /// <returns>The to async.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        /// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
        protected async Task PushAsync(string uri, INavigationParameters parameters = null, bool animated = true)
        {
            //todo: provide a show 'busy indicator' parameter
            try
            {
                /* ==================================================================================================
                 * async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
                 * attentions: the semaphore always released each call, but the field '_wasGone' does not!
                 * ================================================================================================*/
                await _semaphore.WaitAsync();
                if (_wasGone)
                    return;
                _wasGone = true;
                await _navigationService.NavigateAsync(uri, parameters, animated: animated);
            }
            catch (Exception ex)
            {
                _wasGone = false;
                throw ex;
            }
            finally
            {
                _semaphore.Release();
            }
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
            //todo: provide a show 'busy indicator' parameter
            try
            {
                /* ==================================================================================================
                 * async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
                 * attentions: the semaphore always released each call, but the field '_wasGone' does not!
                 * ================================================================================================*/
                await _semaphore.WaitAsync();
                if (_wasGone)
                    return;
                _wasGone = true;
                await _navigationService.NavigateAsync(uri, parameters, true, animated);
            }
            catch (Exception ex)
            {
                _wasGone = false;
                throw ex;
            }
            finally
            {
                _semaphore.Release();
            }
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
        protected async Task<bool> PopAsync(INavigationParameters parameters, bool animated = true)
        {
            try
            {
                /* ==================================================================================================
                 * async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
                 * attentions: the semaphore always released each call, but the field '_wasGone' does not!
                 * ================================================================================================*/
                await _semaphore.WaitAsync();
                if (_wasGone)
                    return false;
                var rs = await _navigationService.GoBackAsync(parameters, null, animated);
                return rs.Success;
            }
            catch (Exception ex)
            {
                _wasGone = false;
                throw ex;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        protected Task<bool> PopModalAsync(bool animated = true)
        {
            return PopModalAsync(null, animated);
        }

        protected async Task<bool> PopModalAsync(INavigationParameters parameters, bool animated = true)
        {
            try
            {
                /* ==================================================================================================
                 * async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
                 * attentions: the semaphore always released each call, but the field '_wasGone' does not!
                 * ================================================================================================*/
                await _semaphore.WaitAsync();
                if (_wasGone)
                    return false;
                var rs = await _navigationService.GoBackAsync(parameters, true, animated);
                return rs.Success;
            }
            catch (Exception ex)
            {
                _wasGone = false;
                throw ex;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// let's back to root
        /// </summary>
        /// <returns>The back to root async.</returns>
        /// <param name="parameters">Parameters.</param>
        protected Task PopToRootAsync(INavigationParameters parameters = null)
        {
            return _navigationService.GoBackToRootAsync(parameters);
        }

        /// <summary>
        /// where the place on earth i'm in?
        /// </summary>
        /// <returns>The am i.</returns>
        protected string WhereAmI(INavigationService navigationService)
        {
            return navigationService.GetNavigationUriPath();
        }
        #endregion

        #region pre-defined
        protected Task GoToMainPageAsync()
        {
            return NavigationHelper.GoToMainPageAsync(_navigationService);
        }

        protected Task GoToLoginPageAsync()
        {
            return NavigationHelper.GoToLoginPageAsync(_navigationService);
        }
        #endregion
    }
}
