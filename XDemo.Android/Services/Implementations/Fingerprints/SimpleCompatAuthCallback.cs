using Android.Support.V4.Hardware.Fingerprint;
using Java.Lang;
using Javax.Crypto;
using XDemo.Core.Infrastructure.Logging;

namespace XDemo.Droid.Services.Implementations.Fingerprints
{
    internal class SimpleCompatAuthCallback : FingerprintManagerCompat.AuthenticationCallback
    {
        // Can be any byte array, keep unique to application.
        static readonly byte[] SECRET_BYTES = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        public override void OnAuthenticationSucceeded(FingerprintManagerCompat.AuthenticationResult result)
        {
            if (result.CryptoObject.Cipher != null)
            {
                try
                {
                    // Calling DoFinal on the Cipher ensures that the encryption worked.
                    byte[] doFinalResult = result.CryptoObject.Cipher.DoFinal(SECRET_BYTES);

                    // No errors occurred, trust the results.              
                }
                catch (BadPaddingException bpe)
                {
                    // Can't really trust the results.
                    LogCommon.Error(bpe);
                }
                catch (IllegalBlockSizeException ibse)
                {
                    // Can't really trust the results.
                    LogCommon.Error(ibse);
                }
            }
            else
            {
                // No cipher used, assume that everything went well and trust the results.
            }
        }

        public override void OnAuthenticationError(int errMsgId, ICharSequence errString)
        {
            // Report the error to the user. Note that if the user canceled the scan,
            // this method will be called and the errMsgId will be FingerprintState.ErrorCanceled.
        }

        public override void OnAuthenticationFailed()
        {
            // Tell the user that the fingerprint was not recognized.
        }

        public override void OnAuthenticationHelp(int helpMsgId, ICharSequence helpString)
        {
            // Notify the user that the scan failed and display the provided hint.
        }
    }
}
