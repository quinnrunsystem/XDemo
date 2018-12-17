using System.Threading.Tasks;
using Android.Support.V4.Hardware.Fingerprint;
using XDemo.Core.BusinessServices.Interfaces.Hardwares.LocalAuthentications;
using Android.Content;
using Android.Hardware.Fingerprints;
using Android.Runtime;
using XDemo.UI;
using System;

namespace XDemo.Droid.Services.Implementations.Fingerprints
{
    public class FingerprintService : ILocalAuthenticationService
    {
        public bool IsEnrolled()
        {
            var context = Android.App.Application.Context;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                /* ==================================================================================================
                 * android api 23 or higher
                 * ================================================================================================*/
                var manager = context.GetSystemService(Context.FingerprintService) as FingerprintManager;
                var rs = manager.HasEnrolledFingerprints;
                return rs;
            }

            // Using the Android Support Library v4
            var managerCompat = FingerprintManagerCompat.From(context);
            var rs2 = managerCompat.HasEnrolledFingerprints;
            return rs2;
        }

        public bool IsSupported()
        {
            var context = Android.App.Application.Context;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                /* ==================================================================================================
                 * android api 23 or higher
                 * ================================================================================================*/
                var manager = context.GetSystemService(Context.FingerprintService) as FingerprintManager;
                var rs = manager.IsHardwareDetected;
                return rs;
            }

            // Using the Android Support Library v4
            var managerCompat = FingerprintManagerCompat.From(context);
            var rs2 = managerCompat.IsHardwareDetected;
            return rs2;
        }

        public  Task<LocalAuthResult> AuthenticateAsync(string reason)
        {
            // CryptoObjectHelper is described in the previous section.
            var cryptoHelper = new CryptoObjectHelper();

            var context = Android.App.Application.Context;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                /* ==================================================================================================
                 * android api 23 or higher
                 * ================================================================================================*/
                var manager = context.GetSystemService(Context.FingerprintService) as FingerprintManager;
                // cancellationSignal can be used to manually stop the fingerprint scanner. 
                var cancellationSignal = new Android.OS.CancellationSignal();
                var authenticationCallback = new SimpleAuthCallback();
                // Start the fingerprint scanner.
                manager.Authenticate(cryptoHelper.BuildCryptoObject(), cancellationSignal, FingerprintAuthenticationFlags.None, authenticationCallback, null);
            }

            // Using the Android Support Library v4
            var managerCompat = FingerprintManagerCompat.From(context);
            // cancellationSignal can be used to manually stop the fingerprint scanner. 
            var cancellationSignalCompat = new Android.Support.V4.OS.CancellationSignal();
            // AuthenticationCallback is a base class that will be covered later on in this guide.
            var callbackCompat = new SimpleCompatAuthCallback();
            // Start the fingerprint scanner.
            managerCompat.Authenticate(cryptoHelper.BuildCompatCryptoObject(), 0, cancellationSignalCompat, callbackCompat, null);
            //return new LocalAuthResult(false);
            return Task.FromResult(new LocalAuthResult(false));
        }

        public void AuthenticateAndroid(string reason)
        {
            // CryptoObjectHelper is described in the previous section.
            var cryptoHelper = new CryptoObjectHelper();

            var context = Android.App.Application.Context;
            //if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            //{
            //    /* ==================================================================================================
            //     * android api 23 or higher
            //     * ================================================================================================*/
            //    var manager = context.GetSystemService(Context.FingerprintService) as FingerprintManager;
            //    // cancellationSignal can be used to manually stop the fingerprint scanner. 
            //    var cancellationSignal = new Android.OS.CancellationSignal();
            //    var authenticationCallback = new SimpleAuthCallback();
            //    // Start the fingerprint scanner.
            //    manager.Authenticate(cryptoHelper.BuildCryptoObject(), cancellationSignal, FingerprintAuthenticationFlags.None, authenticationCallback, null);
            //    return;
            //}

            // Using the Android Support Library v4
            var managerCompat = FingerprintManagerCompat.From(context);
            // cancellationSignal can be used to manually stop the fingerprint scanner. 
            var cancellationSignalCompat = new Android.Support.V4.OS.CancellationSignal();
            // AuthenticationCallback is a base class that will be covered later on in this guide.
            var callbackCompat = new SimpleCompatAuthCallback();
            // Start the fingerprint scanner.
            managerCompat.Authenticate(cryptoHelper.BuildCompatCryptoObject(), 0, cancellationSignalCompat, callbackCompat, null);
            //return new LocalAuthResult(false);
            //return Task.FromResult(new LocalAuthResult(false));
        }
    }
}
