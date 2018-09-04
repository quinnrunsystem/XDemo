using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XDemo.iOS.Renderers;
using XDemo.UI.Controls.ExtendedElements;

[assembly: ExportRenderer(typeof(TabbedPageCustom), typeof(TabbedPageCustomRenderer))]
namespace XDemo.iOS.Renderers
{
    public class TabbedPageCustomRenderer : TabbedRenderer
    {
        /// <summary>
        /// Views the will appear.
        /// </summary>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (TabBar?.Items != null && Element is TabbedPageCustom tabbedPage)
            {
                for (int i = 0; i < TabBar.Items.Length; i++)
                {
                    UpdateItem(TabBar.Items[i], tabbedPage.Children[i].Icon);
                }

                // iOS 10 or newer
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                {
                    // Set Color Text When Selected Tab
                    TabBar.TintColor = tabbedPage.TextColorSelected.ToUIColor();
                    // Set Color Text When UnSelected Tab
                    TabBar.UnselectedItemTintColor = tabbedPage.TextColorUnselected.ToUIColor();
                }
                else
                {
                    // Set Color Text When Selected Tab
                    UITabBarItem.Appearance.SetTitleTextAttributes(
                        new UITextAttributes
                        {
                            TextColor = tabbedPage.TextColorSelected.ToUIColor()
                        }, UIControlState.Normal);

                    // Set Color Text When UnSelected Tab
                    UITabBarItem.Appearance.SetTitleTextAttributes(
                        new UITextAttributes
                        {
                            TextColor = tabbedPage.TextColorUnselected.ToUIColor()
                        }, UIControlState.Selected);
                }
            }
        }

        /// <summary>
        /// Updates the item.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="icon">Icon.</param>
        private void UpdateItem(UITabBarItem item, string icon)
        {
            if (item == null) return;
            try
            {
                //Change Icon When Selected Tab: Ex rate_2 to rate
                string newIcon = icon.Replace("_2", "");

                if (item.SelectedImage == null) return;
                if (item.SelectedImage.AccessibilityIdentifier == icon) return;

                item.Image = UIImage.FromBundle(icon);

                //Used Icon Color Defaul When Unselected
                item.Image = item.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);

                item.SelectedImage = UIImage.FromBundle(newIcon);

                //Used Icon Color Defaul When Selected
                item.SelectedImage = item.SelectedImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);

                item.SelectedImage.AccessibilityIdentifier = icon;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to set selected icon: " + ex);
            }
        }
    }
}
