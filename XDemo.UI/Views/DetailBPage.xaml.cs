using Xamarin.Forms;
using Xamarin.Forms.Internals;
using XDemo.Core.Infrastructure.Logging;
using XDemo.UI.Controls.ExtendedElements.RadioButton;

namespace XDemo.UI.Views
{
    public partial class DetailBPage : ContentPage
    {
        public DetailBPage()
        {
            InitializeComponent();
            ansPicker.ItemsSource = new[]
            {
                "Red",
                "Blue",
                "Green",
                "Yellow",
                //"Orange"
            };
            //ansPicker.Orientation = StackOrientation.Horizontal;
            //ansPicker.CheckedChanged += ansPicker_CheckedChanged;
            ansPicker.SelectedIndex = 2;
        }

        private void ansPicker_CheckedChanged(object sender, int e)
        {
            var radio = sender as RadioButton;

            if (radio == null || radio.RadId == -1)
            {
                return;
            }
            //DisplayAlert("Info", radio.Text, "OK");
        }

        private void Handle_CheckedChanged(object sender, EventArg<bool> e)
        {
            LogCommon.Info($"Handle_CheckedChanged, new value: {e.Data}");
        }
    }
}
