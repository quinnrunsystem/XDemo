using System.Threading.Tasks;
/* ==================================================================================================
 * Permission setup, add to your info.plist:
 * <key>NSFaceIDUsageDescription</key>
 * <string>Need your face to unlock secrets!</string>
 * ================================================================================================*/

namespace XDemo.Core.BusinessServices.Interfaces.Hardwares.LocalAuthentications
{
    /// <summary>
    /// interact with device fingerprint/faceId
    /// </summary>
    public interface IFingerprintService
    {
        bool IsSupported();
        bool IsDeviceReachable();
        Task<LocalAuthResult> AuthenticateAsync(string reason);
    }
}
