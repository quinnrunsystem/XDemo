using Prism.Commands;
using Prism.Navigation;
using XDemo.UI.ViewModels.Base;

namespace XDemo.UI.ViewModels
{
    public class DetailBPageViewModel : ViewModelBase
    {
        public DelegateCommand btnBack { get; set; }
        public DetailBPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Page B";
            btnBack = new DelegateCommand(() =>
            {
                PopAsync();
            });
        }
    }
}
