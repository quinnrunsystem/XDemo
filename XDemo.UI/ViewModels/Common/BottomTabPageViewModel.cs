using XDemo.UI.ViewModels.Base;
using Prism.Navigation;
using System.Threading.Tasks;

namespace XDemo.UI.ViewModels.Common
{
    public class BottomTabPageViewModel : ViewModelBase
    {
        public BottomTabPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public async Task PushPage(string uri)
        {
            await PopToRootAsync();
            await PushAsync(uri);
        }
    }
}