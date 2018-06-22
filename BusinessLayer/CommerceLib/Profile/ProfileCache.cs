using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using System.Web;
using System.Collections;
using System.Text.RegularExpressions;
using Mediachase.Commerce.Engine.Caching;

namespace Mediachase.Commerce.Profile
{
    /// <summary>
    /// Implements operations for the cache for a catalog.
    /// </summary>
    public sealed class ProfileCache
    {
        private const string _CachePrefix = "ecf-prfl";

        /// <summary>
        /// Creates the cache key.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        public static string CreateCacheKey(params string[] keys)
        {
            return CacheHelper.CreateCacheKey(_CachePrefix, keys);
        }

        /// <summary>
        /// Removes all items from the Cache
        /// </summary>
        public static void Clear()
        {
            CacheHelper.Clear(_CachePrefix);
        }

        /// <summary>
        /// Removes the cache by pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        public static void RemoveByPattern(string pattern)
        {
            CacheHelper.RemoveByPattern(pattern);
        }

        /// <summary>
        /// Removes the specified key from the cache
        /// </summary>
        /// <param name="key">The key.</param>
        public static void Remove(string key)
        {
            CacheHelper.Remove(key);
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
            if (!ProfileConfiguration.Instance.Cache.IsEnabled)
                return;

            CacheHelper.Insert(key, obj, dep, timeframe, priority);
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object Get(string key)
        {
            if (!ProfileConfiguration.Instance.Cache.IsEnabled)
                return null;

            return CacheHelper.Get(key);
        }

        /// <summary>
        /// Gets the lock.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object GetLock(string key)
        {
            if (!ProfileConfiguration.Instance.Cache.IsEnabled)
                return new object();

            return CacheHelper.GetLock(key);
        }
    }
}
