using System;

using Xamarin.Forms;

namespace XDemo.UI.Views
{
    public class MyView : StackLayout
    {
        public MyView()
        {
            BackgroundColor = Color.Yellow;
            var button = new Button
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "dsds",
                BackgroundColor = Color.Red
            };
            var label = new Label
            {
                Text = "adsds",
            };
            Children.Add(button);
            Children.Add(label);
        }
    }
}

