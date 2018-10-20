using Xamarin.Forms;
using XDemo.UI.Views.Base;

namespace XDemo.UI.Views.Common
{
    public partial class LoginPage : ViewBase
    {
        public LoginPage()
        {
            InitializeComponent();
            btnLogin.GradientColors = new Color[] { Color.Black, Color.Orange, Color.Blue };
        }
    }
}
