using System;
using System.Threading.Tasks;
using Akavache;
using System.Reactive.Linq;
using XDemo.Core.Storage.Settings;

namespace XDemo.Core.Storage
{
    public sealed class StorageContext
    {
        private static readonly Lazy<StorageContext> Lazy = new Lazy<StorageContext>(() => new StorageContext());
        public static StorageContext Current => Lazy.Value;
        private readonly IBlobCache _deviceCache;
        private readonly IBlobCache _secureCache;
        public StorageContext()
        {
            // local device cache instance
            _deviceCache = BlobCache.LocalMachine;

            // secure cache instance (use for secure cache, such as: login setting...)
            _secureCache = BlobCache.Secure;
        }

        #region private methods
        /// <summary>
        /// update for secure cache
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="newVal">New value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        void UpdateSecure<T>(string key, T newVal) where T : new()
        {
            _secureCache.InsertObject(key, newVal).GetAwaiter().Wait();
        }
        /// <summary>
        /// get values from secure cache
        /// </summary>
        /// <returns>The or create object secure.</returns>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        T GetOrCreateObjectSecure<T>(string key, T defaultValue) where T : new()
        {
            return _secureCache.GetOrCreateObject(key, () => defaultValue).GetAwaiter().Wait();
        }
        /// <summary>
        /// Update the setting non-secure
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="newVal">New value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        void Update<T>(string key, T newVal) where T : new()
        {
            _deviceCache.InsertObject(key, newVal).GetAwaiter().Wait();
        }
        /// <summary>
        /// Gets the or create object non-secure
        /// </summary>
        /// <returns>The or create object.</returns>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        T GetOrCreateObject<T>(string key, T defaultValue) where T : new()
        {
            return _deviceCache.GetOrCreateObject(key, () => defaultValue).GetAwaiter().Wait();
        }
        #endregion


        #region exposed settings
        //storage key = class fullname + ".key";
        private const string LoginSettingsKey = "XDemo.Core.Storage.Settings.LoginSettings.Key";
        /// <summary>
        /// exmple for using secure cache
        /// </summary>
        /// <value>The login setting.</value>
        public LoginSettings LoginSetting
        {
            get => GetOrCreateObjectSecure(LoginSettingsKey, new LoginSettings());
            set => UpdateSecure(LoginSettingsKey, value);
        }

        private const string ExampleSettingKey= "XDemo.Core.Storage.Settings.ExampleSettingKey.Key";
        /// <summary>
        /// exmple for using non-secure cache
        /// </summary>
        /// <value>The bussiness settings.</value>
        public SampleBussinessSettings BussinessSettings
        {
            get => GetOrCreateObject(ExampleSettingKey, new SampleBussinessSettings());
            set => Update(ExampleSettingKey, value);
        }
        #endregion
    }
}
