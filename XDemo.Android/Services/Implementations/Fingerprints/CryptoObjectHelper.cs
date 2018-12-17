using System;
using Android.Support.V4.Hardware.Fingerprint;
using Android.Security.Keystore;
using Java.Security;
using Javax.Crypto;
using Android.Hardware.Fingerprints;

namespace XDemo.Droid.Services.Implementations.Fingerprints
{
    public class CryptoObjectHelper
    {
        // This can be key name you want. Should be unique for the app.
        static readonly string KeyName = $"vn.quinn.XDemo.{Guid.NewGuid()}";

        // We always use this keystore on Android.
        static readonly string KeyStoreName = "AndroidKeyStore";

        // Should be no need to change these values.
        static readonly string KeyAlgorithm = KeyProperties.KeyAlgorithmAes;
        static readonly string BlockMode = KeyProperties.BlockModeCbc;
        static readonly string EncryptionPadding = KeyProperties.EncryptionPaddingPkcs7;
        static readonly string Transformation = $"{KeyAlgorithm}/{BlockMode}/{EncryptionPadding}";
        readonly KeyStore _keystore;

        public CryptoObjectHelper()
        {
            _keystore = KeyStore.GetInstance(KeyStoreName);
            _keystore.Load(null);
        }

        public FingerprintManagerCompat.CryptoObject BuildCompatCryptoObject()
        {
            Cipher cipher = CreateCipher();
            return new FingerprintManagerCompat.CryptoObject(cipher);
        }
        public FingerprintManager.CryptoObject BuildCryptoObject()
        {
            Cipher cipher = CreateCipher();
            return new FingerprintManager.CryptoObject(cipher);
        }

        Cipher CreateCipher(bool retry = true)
        {
            IKey key = GetKey();
            Cipher cipher = Cipher.GetInstance(Transformation);
            try
            {
                cipher.Init(CipherMode.EncryptMode | CipherMode.DecryptMode, key);
            }
            catch (KeyPermanentlyInvalidatedException e)
            {
                _keystore.DeleteEntry(KeyName);
                if (retry)
                {
                    CreateCipher(false);
                }
                else
                {
                    throw new System.Exception("Could not create the cipher for fingerprint authentication.", e);
                }
            }
            return cipher;
        }

        IKey GetKey()
        {
            IKey secretKey;
            if (!_keystore.IsKeyEntry(KeyName))
            {
                CreateKey();
            }

            secretKey = _keystore.GetKey(KeyName, null);
            return secretKey;
        }

        void CreateKey()
        {
            var keyGen = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, KeyStoreName);
            var keyGenSpec =
                new KeyGenParameterSpec.Builder(KeyName, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                    .SetBlockModes(BlockMode)
                    .SetEncryptionPaddings(EncryptionPadding)
                    .SetUserAuthenticationRequired(true)
                    .Build();
            keyGen.Init(keyGenSpec);
            keyGen.GenerateKey();
        }
    }
}
