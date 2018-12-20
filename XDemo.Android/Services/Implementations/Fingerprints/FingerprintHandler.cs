using System;
using Android;
using Android.Content;
using Android.Hardware.Fingerprints;
using Android.OS;
using Android.Runtime;
using Java.Lang;
using Plugin.CurrentActivity;
using XDemo.Core.BusinessServices.Interfaces.Hardwares.LocalAuthentications;

namespace XDemo.Droid.Services.Implementations.Fingerprints
{
    public interface IFingerprint
    {
        void AuthenticationResult(FingerprintResult result);
    }


    public class FingerprintHandler : FingerprintManager.AuthenticationCallback
    {
        private IFingerprint fingerprint;

        public FingerprintHandler(IFingerprint fingerprint)
        {
            this.fingerprint = fingerprint;
        }

        public CancellationSignal Start(FingerprintManager manager, FingerprintManager.CryptoObject cryptoObject)
        {
            CancellationSignal cancellationSignal = new CancellationSignal();
            if(CrossCurrentActivity.Current.Activity.CheckSelfPermission(Manifest.Permission.UseFingerprint) != Android.Content.PM.Permission.Granted)
            {
                return cancellationSignal;
            }
            manager.Authenticate(cryptoObject, cancellationSignal, 0, this, null);
            return cancellationSignal;
        }

        public override void OnAuthenticationError([GeneratedEnum] FingerprintState errorCode, ICharSequence errString)
        {
            fingerprint?.AuthenticationResult(FingerprintResult.Error);
        }

        public override void OnAuthenticationFailed()
        {
            fingerprint?.AuthenticationResult(FingerprintResult.Failed);
        }

        public override void OnAuthenticationHelp([GeneratedEnum] FingerprintState helpCode, ICharSequence helpString)
        {
            fingerprint?.AuthenticationResult(FingerprintResult.Help);
        }

        public override void OnAuthenticationSucceeded(FingerprintManager.AuthenticationResult result)
        {
            fingerprint?.AuthenticationResult(FingerprintResult.Succeed);
        }
    }
}
