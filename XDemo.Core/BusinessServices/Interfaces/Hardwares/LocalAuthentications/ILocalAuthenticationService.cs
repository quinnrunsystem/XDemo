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
    /// interact with device fingerprint/faceId
    /// </summary>
    public interface ILocalAuthenticationService
    {
        /// <summary>
        /// indicate that the device has compabity hardware
        /// </summary>
        /// <returns><c>true</c>, if supported was ised, <c>false</c> otherwise.</returns>
        bool IsSupported();

        /// <summary>
        /// if hardware supported, check if has fingerprint/faceid was configuarated
        /// </summary>
        /// <returns><c>true</c>, if enrolled was ised, <c>false</c> otherwise.</returns>
        bool IsEnrolled();

        /// <summary>
        /// Authenticates async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="reason">Reason.</param>
        Task<LocalAuthResult> AuthenticateAsync(string reason);
        /// <summary>
        /// for test synchronus call.
        /// todo: remove
        /// </summary>
        /// <param name="reason">Reason.</param>
        void AuthenticateAndroid(string reason);
    }
}
