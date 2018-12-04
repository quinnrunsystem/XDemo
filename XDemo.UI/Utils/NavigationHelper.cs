using System;
using System.Threading.Tasks;
using Prism.Navigation;
using XDemo.UI.ViewModels.Common;
using Xamarin.Forms;
using XDemo.UI.Views.Common;

namespace XDemo.UI.Utils
{
    public static class NavigationHelper
    {
        #region pre-defined
        /// <summary>
        /// go to the login page and clear all navi stacks
        /// </summary>
        /// <returns>The to login page async.</returns>
        /// <param name="navigationService">Navigation service.</param>
        public static async Task GoToLoginPageAsync(INavigationService navigationService)
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
        public static async Task GoToMainPageAsync(INavigationService navigationService)
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

        #endregion
    }
}
