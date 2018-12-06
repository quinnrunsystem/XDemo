using System;
using System.Threading.Tasks;
using Foundation;
using LocalAuthentication;
using UIKit;
using Xamarin.Forms;
using XDemo.Core.BusinessServices.Interfaces.Hardwares.LocalAuthentications;
using XDemo.Core.Shared;

namespace XDemo.iOS.Services.Implementations
{
    public class TouchIdService : IFingerprintService
    {
        public async Task<LocalAuthResult> AuthenticateAsync(string reason)
        {
            var context = new LAContext
            {
                LocalizedFallbackTitle = "Fallback" // iOS 8
            };
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                context.LocalizedCancelTitle = "Cancel"; // iOS 10
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                context.LocalizedReason = reason; // iOS 11
            var rs = await AuthOnMainThread(context, reason);
            context.Dispose();
            return rs;
        }

        /// <summary>
        /// Auths the on main thread.
        /// </summary>
        /// <returns>The on main thread.</returns>
        /// <param name="context">Context.</param>
        /// <param name="reason">Reason can not be null or empty</param>
        private Task<LocalAuthResult> AuthOnMainThread(LAContext context, string reason)
        {
            var tcs = new TaskCompletionSource<LocalAuthResult>();
            var result = new LocalAuthResult(false);

            /* ==================================================================================================
             * indicate not allow null or empty reason
             * ================================================================================================*/
            if (string.IsNullOrWhiteSpace(reason))
            {
                result = new LocalAuthResult(false, "Your reason can not be null or empty");
                tcs.SetResult(result);
                return tcs.Task;
            }

            /* ==================================================================================================
             * indicate the hardware
             * ================================================================================================*/
            if (!context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out NSError authError))
            {
                result = new LocalAuthResult(false, authError?.ToString());
                tcs.SetResult(result);
                return tcs.Task;
            }

            /* ==================================================================================================
             * begin auth
             * ================================================================================================*/
            var nsReason = new NSString(reason);
            var replyHandler = new LAContextReplyHandler((success, error) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    result = new LocalAuthResult(success, error?.ToString());
                    tcs.SetResult(result);
                });
            });
            context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, nsReason, replyHandler);

            return tcs.Task;
        }
        public bool IsDeviceReachable()
        {
            throw new System.NotImplementedException();
        }

        public bool IsSupported()
        {
            throw new System.NotImplementedException();
        }
    }
}
