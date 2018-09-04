﻿using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XDemo.Core.BusinessServices.Interfaces.Common;
using XDemo.Core.Extensions;
using XDemo.UI.ViewModels.Base;
using Prism.Services;
using Prism.Navigation;
using XDemo.Core.Infrastructure.Logging;
using XDemo.UI.Extensions;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using System.Threading;

namespace XDemo.UI.ViewModels.Common
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly ISecurityService _securityService;
        private readonly IPageDialogService _pageDialogService;
        private readonly INavigationService _navigationService;
        private readonly IPhotoService _photoService;

        public LoginPageViewModel(ISecurityService securityService, IPageDialogService pageDialogService, INavigationService navigationService, IPhotoService photoService)
        {
            _photoService = photoService;
            _securityService = securityService;
            _pageDialogService = pageDialogService;
            _navigationService = navigationService;

            Title = "Login";
#if DEBUG
            UserName = "nv1";
            Password = "123456";
#endif
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        #region LoginCommand
        private ICommand _loginCommand;
        /// <summary>
        /// Gets the LoginCommand command.
        /// </summary>
        public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new Command(async () => { await LoginCommandExecute(); }, CanExecuteLoginCommand));

        private bool CanExecuteLoginCommand()
        {
            return !UserName.IsNullOrEmpty() && !Password.IsNullOrEmpty() && !IsBusy;
        }

        /// <summary>
        /// Method to invoke when the command LoginCommand is executed.
        /// </summary>
        private async Task LoginCommandExecute()
        {
            try
            {
                IsBusy = true;
                var photos = await _photoService.Get(CancellationToken.None);
                LogCommon.Info($"Photo count: {photos.Items.Count}");
                var pt = await _photoService.Get(3);
                LogCommon.Info($"Photo id: {pt?.Id ?? 0}");
                //var rs = await _securityService.Login(UserName, Password);
                //IsBusy = false;
                //if (!rs.IsValid)
                //{
                //    await _pageDialogService.DisplayAlertAsync("Warning", "Invalid username or password", "Ok");
                //    return;
                //}
                //await _navigationService.GoToMainPage();
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

    }
}