using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using Xamarin.Forms;
using XDemo.Core.BusinessServices.Interfaces.Common;
using XDemo.UI.ViewModels.Base;
using Prism.Services;
using XDemo.UI.Extensions;
using XDemo.UI.Views.Base;
using XDemo.Core.Infrastructure.Logging;

namespace XDemo.UI.ViewModels.Common
{
    public class SettingPageViewModel : ViewModelBase
    {
        private readonly ISecurityService _securityService;
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _pageDialogService;

        public SettingPageViewModel(ISecurityService securityService, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            _securityService = securityService;
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;
        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            Title = "Settings";
            CurrentUser = _securityService.CurrentUser();
            base.OnNavigatingTo(parameters);
        }
        #region LogoutCommand

        private ICommand _logoutCommand;

        public ICommand LogoutCommand => _logoutCommand ?? (_logoutCommand = new Command(async () => { await LogoutCommandExecute(); }));

        public LoginResultDto CurrentUser { get; private set; }

        private async Task LogoutCommandExecute()
        {
            var mrs = await _pageDialogService.DisplayAlertAsync("Confirm", "Are you sure to logout?", "Ok", "Cancel");
            if (!mrs)
                return;
            _securityService.Logout();
            await _navigationService.GoToLoginPageAsync();
        }

        #endregion

        public override void OnScreenRotated(ScreenOrientation orientation)
        {
            base.OnScreenRotated(orientation);
            LogCommon.Info($"Screen rotated, new orientation: {orientation}");
            var mainpage = Application.Current.MainPage;
        }
    }
}
