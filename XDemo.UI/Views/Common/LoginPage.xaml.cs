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
            btnGrandient.GradientColors = new Color[] { Color.Red, Color.Green, Color.Blue };
            btnGrandient.Clicked += OnButtonGrandientClicked;
        }

        private void OnButtonGrandientClicked(object sender, EventArgs e)
        {
            if (!(sender is ExtendedButton btn))
                return;
            btn.GradientFlow = btn.GradientFlow == ExtendedButton.Flows.LeftToRight ? ExtendedButton.Flows.TopDown : ExtendedButton.Flows.LeftToRight;
        }
    }
}
