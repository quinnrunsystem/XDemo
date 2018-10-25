using Foundation;
using UIKit;
using XDemo.UI;
using FFImageLoading.Forms.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using XDemo.Core.Infrastructure.Logging;

namespace XDemo.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        /* ==================================================================================================
         * This method is invoked when the application has loaded and is ready to run. In this
         * method you should instantiate the window, load the UI into it and then make the window visible.
         * 
         * You have 17 seconds to return from this method, or iOS will terminate your application.
         * ================================================================================================*/
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            global::Xamarin.Forms.Forms.Init();
            CachedImageRenderer.Init();
            LoadApplication(new App(new IOSPlatformInitializer()));

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        /* ==================================================================================================
         * this view will blur our app screen in app switcher (see vietcommbank app - iOS)
         * ================================================================================================*/
        UIVisualEffectView _blurView = null;
        public override void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);
            /* ==================================================================================================
             * App is activated => remove the blur view
             * ================================================================================================*/
            try
            {
                if (_blurView != null)
                {
                    _blurView.RemoveFromSuperview();
                    _blurView.Dispose();
                    _blurView = null;
                }
            }
            catch (Exception ex)
            {
                /* ==================================================================================================
                 * ignore any exception if occured
                 * ================================================================================================*/
                LogCommon.Error(ex);
            }
        }

        public override void OnResignActivation(UIApplication uiApplication)
        {
            using (var blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Regular))
            {
                /* ==================================================================================================
                 * init the blur-effect view
                 * ================================================================================================*/
                _blurView = new UIVisualEffectView(blurEffect)
                {
                    Frame = Window.RootViewController.View.Bounds
                };
                /* ==================================================================================================
                 * adding it into main window
                 * ================================================================================================*/
                Window.RootViewController.View.AddSubview(_blurView);
            }
        }
    }
}
