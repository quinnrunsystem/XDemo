using System;
using System.Threading.Tasks;
using Prism.Navigation;
using XDemo.UI.ViewModels.Common;
using Xamarin.Forms;
using XDemo.UI.ViewModels.Base;
using System.Threading;
using XDemo.UI.ViewModels;

namespace XDemo.UI.Extensions
{
    public static class NavigationExtension
    {
        public static INavigationService NavigationServiceTabbar { get; set; }

        #region pre-defined
        /// <summary>
        /// go to the login page and clear all navi stacks
        /// </summary>
        /// <returns>The to login page async.</returns>
        /// <param name="navigationService">Navigation service.</param>
        public static async Task GoToLoginPageAsync(this INavigationService navigationService)
        {
            if (navigationService == null)
                throw new ArgumentNullException(nameof(navigationService));
            await navigationService.NavigateAsync($"/{nameof(LoginPageViewModel)}");
        }

        /// <summary>
        /// Go to main page and clear all nav stack
        /// </summary>
        /// <returns>The to main page.</returns>
        /// <param name="navigationService">Navigation service.</param>
        public static async Task GoToMainPage(this INavigationService navigationService)
        {
            if (navigationService == null)
                throw new ArgumentNullException(nameof(navigationService));
            var navParams = new NavigationParameters
            {
                { KnownNavigationParameters.CreateTab, nameof(HomePageViewModel) },
                { KnownNavigationParameters.CreateTab, nameof(TransactionPageViewModel) },
                { KnownNavigationParameters.CreateTab, nameof(SettingPageViewModel) }
            };
            /* ==================================================================================================
             * using query string instead of navigation parameters, bc of this prism version limitation!
             * ================================================================================================*/
            var query = $"/{nameof(MenuPageViewModel)}/{nameof(NavigationPage)}/{nameof(BottomTabPageViewModel)}{navParams.ToString()}";
            await navigationService.NavigateAsync(query);
        }

        /// <summary>
        /// Go to detail page for Menu.
        /// </summary>
        /// <returns>The to detail page.</returns>
        /// <param name="uri">URI.</param>
        public static async Task GoToDetailPageMenu(string uri)
        {
            if (Application.Current.MainPage is MasterDetailPage master)
            {
                // Close Menu when Open detail page
                master.IsPresented = false;
                // Disable swipe menu when has opened the detail page
                master.IsGestureEnabled = false;
            }
            await NavigationServiceTabbar.PushAsync(uri);
        }
        #endregion

        #region methods
        public static void ResetGone()
        {
            _wasGone = false;
        }

        /// <summary>
        /// The semaphore. <para/>
        /// In case user press a back button in our app and the back button on device (android) or swipeback gesture (iOS) at same time. <para/>
        /// </summary>
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private static bool _wasGone;

        #region Push async
        /// <summary>
        /// navigate to a view model async
        /// </summary>
        /// <returns>The to async.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        /// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
        public static async Task PushAsync<TViewModel>(this INavigationService navigationService, NavigationParameters parameters, bool animated = true) where TViewModel : ViewModelBase
        {
            //todo: provide a show 'busy indicator' parameter
            try
            {
                /* ==================================================================================================
                 * Dont allow null of navigation service sender => throw a managed exception
                 * ================================================================================================*/
                if (navigationService == null)
                    throw new ArgumentNullException(nameof(navigationService));

                /* ==================================================================================================
                 * async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
                 * attentions: the semaphore always released each call, but the field '_wasGone' does not!
                 * ================================================================================================*/
                await _semaphore.WaitAsync();
                if (_wasGone)
                    return;
                _wasGone = true;
                await navigationService.NavigateAsync(typeof(TViewModel).Name, parameters, animated: animated);
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
        /// navigate to a view model async
        /// </summary>
        /// <returns>The to async.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        /// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
        public static async Task PushAsync(this INavigationService navigationService, string uri, NavigationParameters parameters = null, bool animated = true)
        {
            //todo: provide a show 'busy indicator' parameter
            try
            {
                //dont allow null of navigation service sender => throw managed exception
                if (navigationService == null)
                    throw new ArgumentNullException(nameof(navigationService));
                // async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
                // attentions: the semaphore always released each call, but the field '_wasGone' does not!
                await _semaphore.WaitAsync();
                if (_wasGone)
                    return;
                _wasGone = true;
                await navigationService.NavigateAsync(uri, parameters, animated: animated);
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
        /// navigate to a viewmodel async without navigation parameters
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="navigationService">Navigation service.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        /// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
        public static async Task PushAsync<TViewModel>(this INavigationService navigationService, bool animated = true) where TViewModel : ViewModelBase
        {
            await navigationService.PushAsync<TViewModel>(null, animated);
        }
        #endregion

        #region push modal
        /// <summary>
        /// Navigate to a view model with modal mode
        /// </summary>
        /// <returns>The modal async.</returns>
        /// <param name="navigationService">Navigation service.</param>
        /// <param name="parameters">Parameters.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        /// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
        public static async Task PushModalAsync<TViewModel>(this INavigationService navigationService, NavigationParameters parameters, bool animated = true) where TViewModel : ViewModelBase
        {
            //todo: provide a show 'busy indicator' parameter
            try
            {
                /* ==================================================================================================
                 * Dont allow null of navigation service sender => throw a managed exception
                 * ================================================================================================*/
                if (navigationService == null)
                    throw new ArgumentNullException(nameof(navigationService));

                /* ==================================================================================================
                 * async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
                 * attentions: the semaphore always released each call, but the field '_wasGone' does not!
                 * ================================================================================================*/
                await _semaphore.WaitAsync();
                if (_wasGone)
                    return;
                _wasGone = true;
                await navigationService.NavigateAsync(typeof(TViewModel).Name, parameters, true, animated);
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
        /// Navigate to a view model with modal mode, without any navigation parameters
        /// </summary>
        /// <returns>The modal async.</returns>
        /// <param name="navigationService">Navigation service.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        /// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
        public static async Task PushModalAsync<TViewModel>(this INavigationService navigationService, bool animated = true) where TViewModel : ViewModelBase
        {
            await navigationService.PushModalAsync<TViewModel>(null, animated);
        }
        #endregion

        /// <summary>
        /// let's back
        /// </summary>
        /// <returns>The back async.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="useModalNavigate">Use modal navigate.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public static async Task<bool> PopAsync(this INavigationService navigationService, NavigationParameters parameters, bool? useModalNavigate = null, bool animated = true)
        {
            try
            {
                /* ==================================================================================================
                * Dont allow null of navigation service sender => throw a managed exception
                * ================================================================================================*/
                if (navigationService == null)
                    throw new ArgumentNullException(nameof(navigationService));

                /* ==================================================================================================
                 * async lock: we must use this to avoid many navigate commands executed in a same time (i.e: user tap on UI quickly...)
                 * attentions: the semaphore always released each call, but the field '_wasGone' does not!
                 * ================================================================================================*/
                await _semaphore.WaitAsync();
                if (_wasGone)
                    return false;
                return await navigationService.GoBackAsync(parameters, useModalNavigate, animated);
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
        /// let's back
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="navigationService">Navigation service.</param>
        /// <param name="useModalNavigate">Use modal navigate.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public static Task<bool> PopAsync(this INavigationService navigationService, bool? useModalNavigate = null, bool animated = true)
        {
            return navigationService.PopAsync(null, useModalNavigate, animated);
        }

        /// <summary>
        /// let's back to root
        /// </summary>
        /// <returns>The back to root async.</returns>
        /// <param name="parameters">Parameters.</param>
        public static Task PopToRootAsync(this INavigationService navigationService, NavigationParameters parameters = null)
        {
            return navigationService.GoBackToRootAsync(parameters);
        }

        /// <summary>
        /// where the place on earth i'm in?
        /// </summary>
        /// <returns>The am i.</returns>
        public static string WhereAmI(this INavigationService navigationService)
        {
            return navigationService.GetNavigationUriPath();
        }
        #endregion
    }
}
