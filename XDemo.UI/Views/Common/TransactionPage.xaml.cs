using XDemo.UI.Views.Base;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.PlatformConfiguration;

namespace XDemo.UI.Views.Common
{
    public partial class TransactionPage : ViewBase
    {
        public TransactionPage()
        {
            InitializeComponent();
            Icon = "rate_2";
            mainListView.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
       }
    }
}
