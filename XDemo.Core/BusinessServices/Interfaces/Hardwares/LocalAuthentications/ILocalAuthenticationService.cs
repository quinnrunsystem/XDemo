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
        bool IsHardwareSupported();
        Task<LocalAuthResult> AuthenticateAsync(string reason);
    }
}
