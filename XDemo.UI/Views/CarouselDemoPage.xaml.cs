using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XDemo.UI.Controls.ExtendedElements;
using XDemo.UI.Views.Base;

namespace XDemo.UI.Views
{
    public partial class CarouselDemoPage : ViewBase
    {
        public CarouselDemoPage()
        {
            InitializeComponent();
        }
        void CarouselView_ItemSelected(object sender, OffsetCarouselView.CarouselViewSelectedItemChangedEventArgs e)
        {
            if (e.SelectedObject is string imgString)
                selectedImage.Source = imgString;
            selectedLabel.Text = $"Text from ItemSelected event- SelectedIndex = {e.SelectedIndex}";
        }
        public string[] Source { get; } =
        {
            "img_1.png",
            "img_2.png",
            "img_3.png",
            "img_4.png",
        };
    }
}
