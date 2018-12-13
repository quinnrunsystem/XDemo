using Prism.Commands;
using Prism.Navigation;
using XDemo.UI.ViewModels.Base;
using System.Threading.Tasks;
using Xamarin.Forms;
using XDemo.UI.Views.Common;

namespace XDemo.UI.ViewModels.Common
{
    public class MenuPageViewModel : ViewModelBase
    {
        public MenuPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}
