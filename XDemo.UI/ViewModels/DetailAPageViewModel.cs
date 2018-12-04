using Prism.Commands;
using Prism.Navigation;
using XDemo.UI.ViewModels.Base;

namespace XDemo.UI.ViewModels
{
    public class DetailAPageViewModel : ViewModelBase
    {
        public DelegateCommand btnBack { get; set; }

        public DetailAPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Page A";
            btnBack = new DelegateCommand(() =>
            {
                PopAsync();
            });
        }
    }
}
