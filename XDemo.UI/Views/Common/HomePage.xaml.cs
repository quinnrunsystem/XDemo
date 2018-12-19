using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using XDemo.UI.Views.Base;

namespace XDemo.UI.Views.Common
{
    public partial class HomePage : ViewBase
    {
        public HomePage()
        {
            InitializeComponent();
            Icon = "login_2";
            mainListView.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
        }
    }
}
