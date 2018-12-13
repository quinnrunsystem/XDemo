using System;
using Prism.Navigation;
using Xamarin.Forms;
using XDemo.UI.ViewModels.Common;
using XDemo.UI.ViewModels;

namespace XDemo.UI.Views.Common
{
    public partial class MenuPage : MasterDetailPage, IMasterDetailPageOptions
    {
        public MenuPage()
        {
            InitializeComponent();
            buttonToViewA.Clicked += OnMenuButtonClickedAsync;
            buttonToViewB.Clicked += OnMenuButtonClickedAsync;
            buttonToViewRefresh.Clicked += OnMenuButtonClickedAsync;
        }

        private async void OnMenuButtonClickedAsync(object sender, EventArgs e)
        {
            var tabbedRoot = (Detail as NavigationPage)?.RootPage as BottomTabPage;
            if (!(tabbedRoot?.BindingContext is BottomTabPageViewModel vm))
                return;
            if (sender.Equals(buttonToViewA))
            {
                await vm.PushPage(nameof(DetailAPageViewModel));
                IsPresented = false;
            }
            else if (sender.Equals(buttonToViewB))
            {
                await vm.PushPage(nameof(DetailBPageViewModel));
                IsPresented = false;
            }
            else if (sender.Equals(buttonToViewRefresh))
            {
                await vm.PushPage(nameof(RefreshablePageViewModel));
                IsPresented = false;
            }
        }

        public bool IsPresentedAfterNavigation => false;
    }
}
