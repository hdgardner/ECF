using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading;

namespace Mediachase.Commerce.Engine.Caching
{
    public class CacheHelper
    {
        private static readonly Cache _cache;
        /// <summary>
        /// Initializes the <see cref="CacheHelper"/> class.
        /// </summary>
        static CacheHelper()
        {
            HttpContext context = HttpContext.Current;

            if (context != null)
            {
                _cache = context.Cache;
            }
            else
            {
                _cache = HttpRuntime.Cache;
            }
        }

        /// <summary>
        /// Creates the cache key.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        public static string CreateCacheKey(string prefix, params string[] keys)
        {
            StringBuilder returnKey = new StringBuilder(prefix);
            foreach (string key in keys)
            {
                returnKey.Append("-");
                returnKey.Append(key);
            }

            return returnKey.ToString();
        }

        /// <summary>
        /// Removes all items from the Cache
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        public static void Clear(string prefix)
        {
            RemoveByPattern(prefix);
        }

        /// <summary>
        /// Removes the cache by pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        public static void RemoveByPattern(string pattern)
        {
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            while (CacheEnum.MoveNext())
            {
                if (regex.IsMatch(CacheEnum.Key.ToString()))
                    _cache.Remove(CacheEnum.Key.ToString());
            }
        }

        /// <summary>
        /// Removes the specified key from the cache
        /// </summary>
        /// <param name="key">The key.</param>
        public static void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="timespan">The timespan.</param>
        public static void Insert(string key, object obj, TimeSpan timespan)
        {
            Insert(key, obj, null, timespan);
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="timespan">The timespan.</param>
        /// <param name="priority">The priority.</param>
        public static void Insert(string key, object obj, TimeSpan timespan, CacheItemPriority priority)
        {
            Insert(key, obj, null, timespan, priority);
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="dep">The dep.</param>
        /// <param name="timespan">The timespan.</param>
        public static void Insert(string key, object obj, CacheDependency dep, TimeSpan timespan)
        {
            Insert(key, obj, dep, timespan, CacheItemPriority.Normal);
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="dep">The dep.</param>
        /// <param name="timeframe">The timeframe.</param>
        /// <param name="priority">The priority.</param>
        public static void Insert(string key, object obj, CacheDependency dep, TimeSpan timeframe, CacheItemPriority priority)
        {
            CacheItemRemovedCallback callback = null;
            
            // only need call back if item is in the locking states
            if(CacheEntries.ContainsKey(key))
                callback = new CacheItemRemovedCallback(CacheHelper.ItemRemovedCallback);

            Insert(key, obj, dep, timeframe, priority, callback);
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="dep">The dep.</param>
        /// <param name="timeframe">The timeframe.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="callback">The callback.</param>
        public static void Insert(string key, object obj, CacheDependency dep, TimeSpan timeframe, CacheItemPriority priority, CacheItemRemovedCallback callback)
        {
            if (obj != null)
            {                
                _cache.Insert(key, obj, dep, DateTime.Now.Add(timeframe), Cache.NoSlidingExpiration, priority, callback);
            }
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object Get(string key)
        {
            return _cache[key];
        }

        /// <summary>
        /// Gets the lock.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object GetLock(string key)
        {
            return CacheEntries.GetLock(key);
        }

        /// <summary>
        /// Items the removed callback.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="reason">The reason.</param>
        internal static void ItemRemovedCallback(string key, object value, CacheItemRemovedReason reason)
        {
            if (reason == CacheItemRemovedReason.Expired)
            {
                CacheEntry cacheEntry = CacheEntries.Get(key);

                if (cacheEntry != null)
                {
                    lock (cacheEntry.Lock)
                    {
                        CacheEntries.Remove(key);
                    }
                }
            }
        }
    }
}
