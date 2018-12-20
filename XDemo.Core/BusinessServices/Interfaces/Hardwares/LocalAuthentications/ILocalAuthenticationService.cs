using System.Threading.Tasks;
/* ==================================================================================================
 * iOS setup, add to your info.plist:
 * <key>NSFaceIDUsageDescription</key>
 * <string>Need your face to unlock secrets!</string>
 * =======
 * Android settup:
 * <uses-permission android:name="android.permission.USE_FINGERPRINT" />
 * <uses-permission android:name="com.samsung.android.providers.context.permission.WRITE_USE_APP_FEATURE_SURVEY" />
 * ================================================================================================*/

namespace XDemo.Core.BusinessServices.Interfaces.Hardwares.LocalAuthentications
{
    /// <summary>
    /// Fingerprint result.
    /// </summary>
    public enum FingerprintResult
    {
        /// <summary>
        /// The succeed.
        /// </summary>
        Succeed,
        /// <summary>
        /// The failed.
        /// </summary>
        Failed,
        /// <summary>
        /// The help.
        /// </summary>
        Help,
        /// <summary>
        /// The error.
        /// </summary>
        Error
    }

    /// <summary>
    /// Local authentication.
    /// </summary>
    public interface ILocalAuthentication
    {
        /// <summary>
        /// Authentications the fingerprint result.
        /// </summary>
        /// <param name="result">Result.</param>
        void AuthenticationFingerprintResult(FingerprintResult result);
    }

    /// <summary>
    /// interact with device fingerprint/faceId
    /// </summary>
    public interface ILocalAuthenticationService
    {
        /// <summary>
        /// Setlocals the authentication.
        /// </summary>
        /// <param name="localAuthentication">Local authentication.</param>
        void setlocalAuthentication(ILocalAuthentication localAuthentication);

        /// <summary>
        /// indicate that the device has compabity hardware
        /// </summary>
        /// <returns><c>true</c>, if supported was ised, <c>false</c> otherwise.</returns>
        bool IsSupported();

        /// <summary>
        /// for test synchronus call.
        /// todo: remove
        /// </summary>
        /// <param name="reason">Reason.</param>
        void AuthenticFingerprint(string reason);

        /// <summary>
        /// Cancels the authenticate.
        /// </summary>
        void CancelAuthenticate();
    }
}
