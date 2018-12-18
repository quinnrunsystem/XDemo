using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XDemo.UI.Views.Base;
using XDemo.UI.Controls.ExtendedElements;
using XDemo.Core.Infrastructure.Logging;

namespace XDemo.UI.Views.Common
{
    public partial class PhotoDetailPage : ViewBase
    {
        public PhotoDetailPage()
        {
            InitializeComponent();
            picker.SelectedIndexChanged += OnPickerSelectedIndexChanged;
            picker.ItemSelected += OnItemSelected;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(sender is ExtendedPicker orgPicker))
                return;
            LogCommon.Info($"Picker OnItemSelected");
        }

        private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(sender is ExtendedPicker orgPicker))
                return;
            LogCommon.Info($"Picker selected value: {orgPicker.SelectedValue}");
        }
    }
}
