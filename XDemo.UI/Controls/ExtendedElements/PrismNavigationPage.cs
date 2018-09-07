using System;
using Prism.Common;
using Xamarin.Forms;

namespace XDemo.UI.Controls.ExtendedElements
{
    public class PrismNavigationPage : NavigationPage
    {
        public PrismNavigationPage()
        {
            this.Popped += OnPopped;
        }

        private void OnPopped(object sender, NavigationEventArgs e)
        {
            if (Application.Current.MainPage is MasterDetailPage master)
            {
                // Disable swipe menu when has opened the detail page
                master.IsGestureEnabled = true;
            }

            // useModalNavigation = false, because in app used Navigate of Prism useModalNavigation = false
            Page previousPage = PageUtilities.GetOnNavigatedToTarget(e.Page, Application.Current.MainPage, false);
            // Because if use Navigation.PushModalAsync, then Swipe back not call OnNavigatedTo in ViewModel
            // In app, Only chart streamming used Navigation.PushModalAsync (MultiChartScreen.xaml.cs, method ShowChart(..)
            PageUtilities.OnNavigatedTo(previousPage, new Prism.Navigation.NavigationParameters());
        }
    }
}
