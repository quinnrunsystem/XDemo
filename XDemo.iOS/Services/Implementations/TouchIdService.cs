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
        public async Task<LocalAuthResult> AuthenticateAsync(string reason)
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
            var rs = await AuthOnMainThreadAsync(context, reason);
            context.Dispose();
            return rs;
        }

        /// <summary>
        /// Auths the on main thread.
        /// </summary>
        /// <returns>The on main thread.</returns>
        /// <param name="context">Context.</param>
        /// <param name="reason">Reason can not be null or empty</param>
        private Task<LocalAuthResult> AuthOnMainThreadAsync(LAContext context, string reason)
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
            var evaluateTask = context.EvaluatePolicyAsync(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, nsReason);
            evaluateTask.ContinueWith(t =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var rs = t.Result;
                    result = new LocalAuthResult(rs.Item1, rs.Item2?.ToString());
                    tcs.SetResult(result);
                });
            });
            return tcs.Task;
        }

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

        public bool IsEnrolled()
        {
            throw new System.NotImplementedException();
        }

        public void AuthenticateAndroid(string reason)
        {
            throw new System.NotImplementedException("Do not support on iOS");
        }
    }
}
