using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using System.Web;
using System.Collections;
using System.Text.RegularExpressions;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Implements operations for the order cache.
    /// </summary>
    public sealed class OrderCache
    {
		private const string _CachePrefix = "ecf-ord";

        private static readonly Cache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCache"/> class.
        /// </summary>
        private OrderCache() { }


        /// <summary>
        /// Initializes the <see cref="OrderCache"/> class.
        /// </summary>
        static OrderCache()
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
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        public static string CreateCacheKey(params string[] keys)
        {
            StringBuilder returnKey = new StringBuilder(_CachePrefix);
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
        public static void Clear()
        {
            RemoveByPattern(_CachePrefix);
            /*
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            while (CacheEnum.MoveNext())
                _cache.Remove(CacheEnum.Key.ToString());
             * */
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
            if (!OrderConfiguration.Instance.Cache.IsEnabled)
                return;

            if (obj != null)
            {
                _cache.Insert(key, obj, dep, DateTime.Now.Add(timeframe), Cache.NoSlidingExpiration, priority, null);
            }

        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object Get(string key)
        {
            if (!OrderConfiguration.Instance.Cache.IsEnabled)
                return null;

            return _cache[key];
        }
    }
}
