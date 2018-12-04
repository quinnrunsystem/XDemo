using System.Windows.Input;
using Xamarin.Forms;
using XDemo.UI.ViewModels.Base;
using System.Threading.Tasks;
using Prism.Navigation;

namespace XDemo.UI.ViewModels.Common
{
    public class ChangePasswordPopupPageViewModel : ViewModelBase
    {
        public ChangePasswordPopupPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
        #region BackCommand

        private ICommand _backCommand;

        public ICommand BackCommand => _backCommand ?? (_backCommand = new Command(async () => await BackCommandExecute()));

        private async Task BackCommandExecute()
        {
            await PushAsync<ChangePasswordPopupPageViewModel>();
        }

        #endregion
    }
}
