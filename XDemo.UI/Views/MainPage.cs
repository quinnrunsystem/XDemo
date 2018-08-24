using System;
using Xamarin.Forms;
using XDemo.UI.Views.Base;
namespace XDemo.UI.Views
{
    public class MainPage : ViewBase
    {
        public MainPage()
        {
            Content = new Label
            {
                Text = "Hello Xamarin.Forms!",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }
    }
}
