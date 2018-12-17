using System;
using Xamarin.Forms;
using XDemo.UI.Views.Base;
using XDemo.UI.Controls.ExtendedElements;

namespace XDemo.UI.Views.Common
{
    public partial class LoginPage : ViewBase
    {
        public LoginPage()
        {
            InitializeComponent();
            btnGrandient.GradientColors = new Color[] { Color.FromRgb(247, 12, 27), Color.FromRgb(177, 6, 16) };
            btnGrandient.GradientFlow = ExtendedButton.Flows.TopDown;
            btnGrandient.Clicked += OnButtonGrandientClicked;
            animatedText.Text = "Welcome to Xamarin";
            animatedText.FontSize = 20;
            animatedText.FontAttributes = FontAttributes.Bold;
        }

        private void OnButtonGrandientClicked(object sender, EventArgs e)
        {
            if (!(sender is ExtendedButton btn))
                return;
            btn.GradientFlow = btn.GradientFlow == ExtendedButton.Flows.LeftToRight ? ExtendedButton.Flows.TopDown : ExtendedButton.Flows.LeftToRight;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            animatedText.IsRunning = true;
        }
    }
}
