using System.Collections.Generic;
using Android.Graphics;

namespace XDemo.Droid.Extensions
{
    /// <summary>
    /// Default implementation of the typeface cache.
    /// </summary>
    internal class DefaultTypefaceCache : ITypefaceCache
    {
        private Dictionary<string, Typeface> _cacheDict;

        public DefaultTypefaceCache()
        {
            _cacheDict = new Dictionary<string, Typeface>();
        }


        public Typeface RetrieveTypeface(string key)
        {
            if (_cacheDict.ContainsKey(key))
            {
                return _cacheDict[key];
            }
            else
            {
                return null;
            }
        }

        public void StoreTypeface(string key, Typeface typeface)
        {
            _cacheDict[key] = typeface;
        }

        public void RemoveTypeface(string key)
        {
            _cacheDict.Remove(key);
        }

        public void PurgeCache()
        {
            _cacheDict = new Dictionary<string, Typeface>();
        }
    }
}