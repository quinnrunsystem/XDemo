using System;
using Prism.Navigation;
using XDemo.UI.Extensions;
using XDemo.UI.ViewModels.Base;
using System.Net;

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
