using System;
using Prism.Navigation;
using XDemo.UI.Extensions;
using XDemo.UI.ViewModels.Base;

namespace XDemo.UI.ViewModels.Common
{
    public class BottomTabPageViewModel : ViewModelBase
    {
        public BottomTabPageViewModel(INavigationService navigationService)
        {
            NavigationExtension.NavigationServiceTabbar = navigationService;
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}
