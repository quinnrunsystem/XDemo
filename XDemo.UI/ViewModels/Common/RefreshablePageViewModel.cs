using System.Windows.Input;
using Xamarin.Forms;
using XDemo.UI.ViewModels.Base;
using System.Threading.Tasks;
using Prism.Navigation;

namespace XDemo.UI.ViewModels.Common
{
    public class RefreshablePageViewModel : ViewModelBase
    {
        public RefreshablePageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }
        #region RefreshCommand

        private ICommand _refreshCommand;

        public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new Command(async () => await RefreshCommandExecute()));

        private async Task RefreshCommandExecute()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            await Task.Delay(1500);
            IsBusy = false;
        }

        #endregion
    }
}