using Prism.Commands;
using Prism.Navigation;
using XDemo.UI.ViewModels.Base;
using XDemo.UI.Extensions;

namespace XDemo.UI.ViewModels.Common
{
    public class MenuPageViewModel : ViewModelBase
    {
        public DelegateCommand<string> NavigateCommand { get; set; }
        private readonly INavigationService _navigationService;

        public MenuPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateCommand = new DelegateCommand<string>(HandleAction);
        }

        /// <summary>
        /// Handles the action.
        /// </summary>
        /// <param name="uri">URI.</param>
        private async void HandleAction(string uri)
        {
            await NavigationExtension.GoToDetailPageMenu($"{uri}");
        }
    }
}
