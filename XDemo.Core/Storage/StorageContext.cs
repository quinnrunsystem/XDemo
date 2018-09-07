using System;
using Akavache;
using System.Reactive.Linq;
using XDemo.Core.Storage.Settings;

namespace XDemo.Core.Storage
{
    public sealed class StorageContext
    {
        private static readonly Lazy<StorageContext> Lazy = new Lazy<StorageContext>(() => new StorageContext());
        public static StorageContext Current => Lazy.Value;
        /// <summary>
        /// The device cache. (not encrypted)
        /// </summary>
        private readonly IBlobCache _deviceCache;
        /// <summary>
        /// The secure cache. (encrypted)
        /// </summary>
        private readonly IBlobCache _secureCache;
        private StorageContext()
        {
            /* ==================================================================================================
             * Make the default ctor private to hide it.
             * ================================================================================================*/
            _deviceCache = BlobCache.LocalMachine;
            _secureCache = BlobCache.Secure;
        }

        #region private methods
        /// <summary>
        /// update for secure cache
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="newVal">New value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        void UpdateWithSecure<T>(string key, T newVal) where T : new()
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
        T GetOrCreateObjectWithSecure<T>(string key, T defaultValue) where T : new()
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
        /* ==================================================================================================
         * CONVENTIONS:
         * 1. Storage data must be a class. its better performance the use separate data each key.
         * 2. Storage keys formula: TheKey = <ClassFullName>.Key
         * 3. DONT use the keyword 'nameof', we must hardcode the key.
         * Why?
         * => using 'nameof': our constant can be changed if the parameter changed (in case we rename the class or variable)
         * But when app started once before by user, our keys are now store in outside scope (ie: local database)
         * It may cause malfunctions/bugs...
         * ================================================================================================*/

        private const string LoginSettingsKey = "XDemo.Core.Storage.Settings.LoginSettings.Key";
        /// <summary>
        /// exmple for using secure cache
        /// </summary>
        /// <value>The login setting.</value>
        public LoginSettings LoginSetting
        {
            get => GetOrCreateObjectWithSecure(LoginSettingsKey, new LoginSettings());
            set => UpdateWithSecure(LoginSettingsKey, value);
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
