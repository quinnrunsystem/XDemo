using System.Threading.Tasks;
using Foundation;
using LocalAuthentication;
using UIKit;
using Xamarin.Forms;
using XDemo.Core.BusinessServices.Interfaces.Hardwares.LocalAuthentications;

namespace XDemo.iOS.Services.Implementations
{
    public class TouchIdService : ILocalAuthenticationService
    {
        ILocalAuthentication localAuthentication;

        public bool IsSupported()
        {
            using (var context = new LAContext())
            {
                var result = context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out var error);
                if (result)
                    return result;
                var status = (LAStatus)(int)error.Code;
                result = status != LAStatus.BiometryNotAvailable;
                return result;
            }
        }

        public void AuthenticFingerprint(string reason)
        {
            var context = new LAContext
            {
                LocalizedFallbackTitle = "Fallback" // iOS 8
            };
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                context.LocalizedCancelTitle = "Cancel"; // iOS 10
            }
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                context.LocalizedReason = reason; // iOS 11
            }
            AuthOnMainThread(context, reason);
        }

        private void AuthOnMainThread(LAContext context, string reason)
        {
            var tcs = new TaskCompletionSource<LocalAuthResult>();
            var result = new LocalAuthResult(false);

            /* ==================================================================================================
             * indicate not allow null or empty reason
             * ================================================================================================*/
            if (string.IsNullOrWhiteSpace(reason))
            {
                localAuthentication.AuthenticationFingerprintResult(FingerprintResult.Error);
            }

            /* ==================================================================================================
             * indicate the hardware
             * ================================================================================================*/
            if (!context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out NSError authError))
            {
                localAuthentication.AuthenticationFingerprintResult(FingerprintResult.Help);
            }

            /* ==================================================================================================
             * begin auth
             * ================================================================================================*/
            var nsReason = new NSString(reason);
            var evaluateTask = context.EvaluatePolicyAsync(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, nsReason);
            evaluateTask.ContinueWith(t =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if(t.Result.Item1)
                        localAuthentication.AuthenticationFingerprintResult(FingerprintResult.Succeed);
                    else
                        localAuthentication.AuthenticationFingerprintResult(FingerprintResult.Error);
                });
            });
        }

        public void setlocalAuthentication(ILocalAuthentication localAuthentication)
        {
            this.localAuthentication = localAuthentication;
        }

        public void CancelAuthenticate()
        {
            var alert = UIAlertController.Create("Login fail", "Can not login with fingerprint", UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (topController.PresentedViewController != null)
            {
                topController = topController.PresentedViewController;
            }
            topController.PresentViewController(alert, true, null);
        }
    }
}
