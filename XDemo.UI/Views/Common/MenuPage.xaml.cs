using Prism.Navigation;
using Xamarin.Forms;

namespace XDemo.UI.Views.Common
{
    public partial class MenuPage : MasterDetailPage, IMasterDetailPageOptions
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        public bool IsPresentedAfterNavigation
        {
            get { return Device.Idiom != TargetIdiom.Phone; }
        }
    }
}
