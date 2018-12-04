using Prism.Commands;
using Prism.Navigation;
using XDemo.UI.ViewModels.Base;
using System.Threading.Tasks;

namespace XDemo.UI.ViewModels.Common
{
    public class MenuPageViewModel : ViewModelBase
    {
        public DelegateCommand<string> NavigateCommand { get; set; }

        public MenuPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            NavigateCommand = new DelegateCommand<string>(async (obj) => await HandleAction(obj));
        }

        /// <summary>
        /// Handles the action.
        /// </summary>
        /// <param name="uri">URI.</param>
        private async Task HandleAction(string uri)
        {
            await PushAsync(uri);
        }
    }
}
