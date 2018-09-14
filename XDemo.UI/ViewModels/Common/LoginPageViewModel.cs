using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XDemo.Core.BusinessServices.Interfaces.Common;
using XDemo.UI.ViewModels.Base;
using Prism.Services;
using Prism.Navigation;
using XDemo.UI.Extensions;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using XDemo.Core.Storage;
using XDemo.UI.Models.Validations.Base;
using XDemo.UI.Models.Validations.DefinedRules;

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
            AddValidations();
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            //get value from storage settings
            var setting = StorageContext.Current.LoginSetting;
            //example using local setting
            UserName.Value = setting.SavedUserId;
            Password.Value = setting.SavedPassword;

            /* ==================================================================================================
             * DONT USE LIKE THIS => BC THE SETTING VALUE WILL BE READ MANY TIMES (NOT GOOD)
             * UserName = StorageContext.Current.LoginSetting.SavedUserId;
             * ================================================================================================*/
        }

        public ValidatableObject<string> UserName { get; set; } = new ValidatableObject<string>();

        public ValidatableObject<string> Password { get; set; } = new ValidatableObject<string>();


        #region LoginCommand
        private ICommand _loginCommand;
        /// <summary>
        /// Gets the LoginCommand command.
        /// </summary>
        public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new Command(async () => { await LoginCommandExecute(); }, CanExecuteLoginCommand));

        private bool CanExecuteLoginCommand()
        {
            return true;
        }

        /// <summary>
        /// Method to invoke when the command LoginCommand is executed.
        /// </summary>
        private async Task LoginCommandExecute()
        {
            try
            {
                IsBusy = true;
                //refetch current setting
                var setting = StorageContext.Current.LoginSetting;
                //update setting values
                setting.SavedUserId = UserName.Value;
                setting.SavedPassword = Password.Value;
                //save the new setting values
                StorageContext.Current.LoginSetting = setting;
                var rs = await _securityService.Login(UserName.Value, Password.Value);
                IsBusy = false;
                if (!rs.IsValid)
                {
                    await _pageDialogService.DisplayAlertAsync("Warning", "Invalid username or password", "Ok");
                    return;
                }
                await _navigationService.GoToMainPage();
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        #region ValidateCommand

        private ICommand _validateCommand;

        public ICommand ValidateCommand => _validateCommand ?? (_validateCommand = new Command(ValidateCommandExecute));

        private void ValidateCommandExecute()
        {
            UserName.Validate();
            Password.Validate();
        }

        #endregion

        #region private methods
        void AddValidations()
        {
            /* ==================================================================================================
             * exmple for combining two or more rules for one property
             * ================================================================================================*/
            UserName.Rules.AddRange(new IValidationRule<string>[]
            {
                new RequiredRule<string> { ValidationMessage = "User name can not be blank!" },
                new MinLengthRule<string>(6) { ValidationMessage = "User name is at least 6 characters!" },
                new MaxLengthRule<string>(12) { ValidationMessage = "User name max length is 12 characters!" }
            });

            Password.Rules.AddRange(new IValidationRule<string>[]
            {
                new RequiredRule<string> { ValidationMessage = "Please enter your password!" },
                new MinLengthRule<string>(6) { ValidationMessage = "Password is at least 6 characters" }
            });
         }
        #endregion
    }
}