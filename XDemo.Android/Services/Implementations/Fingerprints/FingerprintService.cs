using System.Threading.Tasks;
using Android.Support.V4.Hardware.Fingerprint;
using XDemo.Core.BusinessServices.Interfaces.Hardwares.LocalAuthentications;
using Android.Content;
using Android.Hardware.Fingerprints;
using Android.Runtime;
using XDemo.UI;
using System;
using Android.Support.V7.App;
using Xamarin.Forms;
using Plugin.CurrentActivity;
using Android;
using Java.Security;
using System.Diagnostics;
using Javax.Crypto;
using Android.Security.Keystore;
using XDemo.Core.Infrastructure.Logging;
using XDemo.UI.ViewModels.Common;
using Android.OS;

namespace XDemo.Droid.Services.Implementations.Fingerprints
{

    public class FingerprintService : ILocalAuthenticationService, IFingerprint
    {
        CancellationSignal _cancellationSignal;
        AlertDialog _alertDialog;
        ILocalAuthentication _localAuthentication;
        KeyStore _keyStore;
        string _androidKeyStore = "AndroidKeyStore";
        string _keyName = "androidHive";
        Cipher _cipher;

        public void setlocalAuthentication(ILocalAuthentication fingerprintVM)
        {
            this._localAuthentication = fingerprintVM;
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
                if (manager.IsHardwareDetected)
                {
                    if (CrossCurrentActivity.Current.Activity.CheckSelfPermission(Manifest.Permission.UseFingerprint) == Android.Content.PM.Permission.Granted)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void GenerateKey()
        {
            try
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
                {
                    _keyStore = KeyStore.GetInstance(_androidKeyStore);
                    KeyGenerator keyGenerator = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, _androidKeyStore);
                    _keyStore.Load(null);
                    keyGenerator.Init(new KeyGenParameterSpec.Builder(_keyName,
                                                                    KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                                                                    .SetBlockModes(KeyProperties.BlockModeCbc)
                                                                    .SetUserAuthenticationRequired(true)
                                                                    .SetEncryptionPaddings(KeyProperties.EncryptionPaddingPkcs7)
                                                                    .Build());
                    keyGenerator.GenerateKey();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private bool CipherInit()
        {
            try
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
                {
                    _cipher = Cipher.GetInstance($"{KeyProperties.KeyAlgorithmAes}/{KeyProperties.BlockModeCbc}/{KeyProperties.EncryptionPaddingPkcs7}");
                    _keyStore.Load(null);
                    var key = _keyStore.GetKey(_keyName, null);
                    _cipher.Init(CipherMode.EncryptMode, key);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public void AuthenticFingerprint(string reason)
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                ShowDialog();

                GenerateKey();

                if (CipherInit())
                {
                    var manager = CrossCurrentActivity.Current.Activity.GetSystemService(Context.FingerprintService) as FingerprintManager;
                    FingerprintManager.CryptoObject cryptoObject = new FingerprintManager.CryptoObject(_cipher);
                    FingerprintHandler fingerprintHandler = new FingerprintHandler(this);
                    _cancellationSignal = fingerprintHandler.Start(manager, cryptoObject);
                }
            }
        }

        public void CancelAuthenticate()
        {
            _cancellationSignal?.Cancel();
            ShowDialogError();
        }

        void ShowDialog()
        {
            _alertDialog = new AlertDialog.Builder(CrossCurrentActivity.Current.Activity).Create();
            _alertDialog.SetTitle("Login");
            _alertDialog.SetMessage("Touch the fingerprint scanner to login");
            _alertDialog.SetButton(-1, "Cancel", (sender, e) =>
            {
                _cancellationSignal?.Cancel();
                _alertDialog.Dismiss();
            });
            _alertDialog.SetCancelable(false);
            _alertDialog.SetIcon(Android.Resource.Drawable.IcSecure);
            _alertDialog.Show();
        }

        void ShowDialogError()
        {
            CrossCurrentActivity.Current.Activity.RunOnUiThread(() =>
            {

                AlertDialog alertDialog = new AlertDialog.Builder(CrossCurrentActivity.Current.Activity).Create();
                alertDialog.SetTitle("Login fail");
                alertDialog.SetMessage("Can not login with fingerprints");
                alertDialog.SetButton(-1, "OK", (sender, e) =>
                {
                    alertDialog.Dismiss();
                });
                alertDialog.SetCancelable(false);
                alertDialog.SetIcon(Android.Resource.Drawable.IcLockPowerOff);
                alertDialog?.Show();
            });
        }


        public void AuthenticationResult(FingerprintResult result)
        {
            if (result == FingerprintResult.Error || result == FingerprintResult.Succeed)
            {
                _alertDialog?.Dismiss();
                _cancellationSignal?.Cancel();

            }

            _localAuthentication?.AuthenticationFingerprintResult(result);
            LogCommon.Info($"AuthenticationResult: {result}");
        }
    }
}
