using Prism.Commands;
using Prism.Navigation;
using Prism.Mvvm;
using XDemo.UI.Extensions;
using XDemo.UI.ViewModels.Base;

namespace XDemo.UI.ViewModels
{
    public class DetailBPageViewModel : ViewModelBase
    {
        public DelegateCommand btnBack { get; set; }
        public DetailBPageViewModel(INavigationService navigationService)
        {
            Title = "Page B";
            btnBack = new DelegateCommand(() => {
                navigationService.PopAsync();
            });
        }
    }
}
