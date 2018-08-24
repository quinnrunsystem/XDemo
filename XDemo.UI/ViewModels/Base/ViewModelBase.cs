using System;
using Xamarin.Forms;
using Prism.Navigation;
using PropertyChanged;
using Prism.AppModel;
using Prism;
using System.Threading.Tasks;
using Prism.Logging;
using XDemo.Core.Infrastructure.Logging;
using XDemo.UI.Views.Base;
using System.Threading;
using System.Collections;

namespace XDemo.UI.ViewModels.Base
{
    [AddINotifyPropertyChangedInterface]
    public abstract class ViewModelBase : BindableObject, INavigationAware, IPageLifecycleAware, IApplicationLifecycleAware, IDestructible
    {
        private readonly INavigationService _navigator;
        private readonly ILogger _logger;

        protected ViewModelBase(INavigationService navigation, ILogger logger)
        {
            _navigator = navigation;
            _logger = logger;
        }

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
            _wasGone = false;
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
        public virtual async Task OnScreenRotatedAsync(ScreenOrientation orientation) { }

        /// <summary>
        /// The semaphore. <para/>
        /// In case user press a back button in our app and the back button on device (android) or swipeback gesture (iOS) at same time. <para/>
        /// </summary>
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _wasGone;
        /// <summary>
        /// navigate to a view model async
        /// </summary>
        /// <returns>The to async.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="useModalNavigate">Use modal navigate.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        /// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
        protected async Task PushAsync<TViewModel>(NavigationParameters parameters, bool animated = true) where TViewModel : ViewModelBase
        {
            try
            {
                await _semaphore.WaitAsync();
                if (_wasGone)
                    return;
                _wasGone = true;
                _logger.Info($"Begin NavigateToAsync: {typeof(TViewModel).Name} - Navigation mode {parameters.GetNavigationMode()}");
                await _navigator.NavigateAsync(typeof(TViewModel).Name, parameters, animated: animated);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        protected async Task PushModalAsync<TViewModel>(NavigationParameters parameters, bool animated = true) where TViewModel : ViewModelBase
        {
            try
            {
                await _semaphore.WaitAsync();
                if (_wasGone)
                    return;
                _wasGone = true;
                _logger.Info($"Begin NavigateToAsync: {typeof(TViewModel).Name} - Navigation mode {parameters.GetNavigationMode()}");
                await _navigator.NavigateAsync(typeof(TViewModel).Name, parameters, true, animated);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// let's back
        /// </summary>
        /// <returns>The back async.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="useModalNavigate">Use modal navigate.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        protected async Task<bool> PopAsync(NavigationParameters parameters, bool? useModalNavigate, bool animated = true)
        {
            try
            {
                await _semaphore.WaitAsync();
                if (_wasGone)
                    return false;
                _wasGone = true;
                _logger.Info($"Begin GoBackAsync from: {GetType().Name}");
                return await _navigator.GoBackAsync(parameters, useModalNavigate, animated);
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
        protected Task PopToRootAsync(NavigationParameters parameters = null)
        {
            return _navigator.GoBackToRootAsync(parameters);
        }

        /// <summary>
        /// where the place on earth i'm in?
        /// </summary>
        /// <returns>The am i.</returns>
        protected string WhereAmI()
        {
            return _navigator.GetNavigationUriPath();
        }
    }
}
