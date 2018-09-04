using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using XDemo.UI.Controls.ExtendedElements;

namespace XDemo.UI.Views.Common
{
    public partial class PrismTabbedPage : TabbedPageCustom
    {
        public PrismTabbedPage()
        {
            InitializeComponent();

            // On Android
            this.On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            this.On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
            this.On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsLegacyColorModeEnabled(false);
            this.On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarItemColor(Color.Yellow); // text color default
            this.On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarSelectedItemColor(Color.Red);  // selected text color
        }
    }
}
