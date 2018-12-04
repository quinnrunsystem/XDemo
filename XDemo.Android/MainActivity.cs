using XDemo.UI;
using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Platform;
using Android.Views;
using Xamarin.Forms;

namespace XDemo.Droid
{
    [Activity(Label = "XDemo", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public partial class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            /* ==================================================================================================
             * use Xamarin.Forms Fast Renderers
             * https://docs.microsoft.com/en-us/xamarin/xamarin-forms/internals/fast-renderers            
             * ================================================================================================*/
            Forms.SetFlags("FastRenderers_Experimental");

            Forms.Init(this, savedInstanceState);

            /* ==================================================================================================
             * init the FFImageLoading component
             * ================================================================================================*/
            CachedImageRenderer.Init(true);

            /* ==================================================================================================
             * set the app info secure in app switcher
             * ================================================================================================*/
            Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);

            /* ==================================================================================================
             * start load the app
             * ================================================================================================*/
            LoadApplication(new App(new AndroidPlatformInitializer()));
        }
    }
}