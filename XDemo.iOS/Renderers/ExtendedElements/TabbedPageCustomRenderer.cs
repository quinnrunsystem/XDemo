using System;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XDemo.iOS.Renderers.ExtendedElements;
using XDemo.UI.Controls.ExtendedElements;

[assembly: ExportRenderer(typeof(TabbedPageCustom), typeof(TabbedPageCustomRenderer))]
namespace XDemo.iOS.Renderers.ExtendedElements
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

        UIView containerView;
        UIButton zz;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            containerView = new UIView()
            {
                Frame = new CoreGraphics.CGRect(0, View.Frame.Height, View.Frame.Width, 0),
                BackgroundColor = UIColor.Purple
            };

            zz = new UIButton
            {
                //Frame = new CoreGraphics.CGRect(View.Frame.Width - 45, this.containerView.Frame.Y - 45, 40, 20),
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            View.AddSubview(containerView);
            View.AddSubview(zz);

            zz.RightAnchor.ConstraintEqualTo(View.RightAnchor, new nfloat(-5)).Active = true;
            zz.BottomAnchor.ConstraintEqualTo(TabBar.BottomAnchor, new nfloat(-10)).Active = true;
            zz.WidthAnchor.ConstraintEqualTo(40).Active = true;
            zz.HeightAnchor.ConstraintEqualTo(25).Active = true;

            zz.SetBackgroundImage(new UIImage("arrow"), UIControlState.Normal);
            //containerView.Frame = new CoreGraphics.CGRect(0, View.Frame.Height, View.Frame.Width, 0);
            //zz.Frame = new CoreGraphics.CGRect(View.Frame.Width - 45, this.View.Frame.Height - 45, 40, 20);
            containerView.Hidden = true;

            zz.AddTarget(HandleEventHandler, UIControlEvent.TouchUpInside);
        }

        void HandleEventHandler(object sender, EventArgs e)
        {
            if (containerView.Hidden) // Show Animation
            {
                containerView.Hidden = false;
                UIView.Animate(
                    duration: 0.3,
                    delay: 0,
                    options: UIViewAnimationOptions.TransitionFlipFromBottom,
                    animation: () =>
                    {
                        containerView.Frame = new CoreGraphics.CGRect(0, this.View.Frame.Height - 250, View.Frame.Width, 250);
                        //zz.Frame = new CoreGraphics.CGRect(View.Frame.Width - 45, containerView.Frame.Y - 45, 40, 20);

                        zz.BottomAnchor.ConstraintEqualTo(containerView.TopAnchor, new nfloat(-5)).Active = true;
                        zz.Transform = CGAffineTransform.Rotate(zz.Transform, (nfloat)Math.PI * 180);
                        zz.Transform = CGAffineTransform.Rotate(zz.Transform, (nfloat)Math.PI);

                    },
                    completion: () =>
                    {
                    }
                );
            }
            else // Hide Animation
            {
                UIView.Animate(
                    duration: 0.3,
                    delay: 0,
                    options: UIViewAnimationOptions.TransitionFlipFromTop,
                    animation: () =>
                    {
                        containerView.Frame = new CoreGraphics.CGRect(0, View.Frame.Height, View.Frame.Width, 0);
                        //zz.Frame = new CoreGraphics.CGRect(View.Frame.Width - 45, this.View.Frame.Height - 45, 40, 20);
                        zz.BottomAnchor.ConstraintEqualTo(TabBar.BottomAnchor, new nfloat(-10)).Active = true;

                        zz.Transform = CGAffineTransform.MakeIdentity();
                    },
                    completion: () =>
                    {

                        containerView.Hidden = true;
                    }
                );
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
