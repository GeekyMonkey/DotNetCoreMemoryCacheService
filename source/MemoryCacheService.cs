using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeekyMonkey.DotNetCore
{
    /// <summary>
    /// Memory Cache Service
    /// </summary>
    public class MemoryCacheService
    {
        /// <summary>
        /// Memory cache
        /// </summary>
        private readonly IMemoryCache MemoryCache;

        /// <summary>
        /// Associate cache group names with cancellation tokens
        /// </summary>
        private ConcurrentDictionary<string, CancellationTokenSource> cancellationGroups = new ConcurrentDictionary<string, CancellationTokenSource>();

        /// <summary>
        /// Email service constructor
        /// </summary>
        public MemoryCacheService(IMemoryCache memoryCache)
        {
            this.MemoryCache = memoryCache;
        }

        /// <summary>
        /// Remove a single item from the cache by it's cache key
        /// </summary>
        /// <param name="cacheKey">Cache Key used when adding the item</param>
        public void RemoveItem(string cacheKey)
        {
            MemoryCache.Remove(cacheKey);
        }

        /// <summary>
        /// Remove items from the cache that were registered with the given group
        /// </summary>
        /// <param name="cacheGroup">Cache group name</param>
        public void ClearCacheGroup(string cacheGroup)
        {
            var token = GetGroupCancellationToken(cacheGroup);
            if (token != null)
            {
                token.Cancel();

                cancellationGroups.TryRemove(cacheGroup, out CancellationTokenSource previousTokenSource);
            }
        }

        /// <summary>
        /// Remove all items from the cache
        /// </summary>
        public void ClearAll()
        {
            List<string> groupNames = cancellationGroups.Keys.ToList();
            foreach(string groupName in groupNames)
            {
                this.ClearCacheGroup(groupName);
            }
        }

        /// <summary>
        /// Get the cancellation token from the dictionary that matches the group name (or null)
        /// </summary>
        /// <param name="cacheGroup">Cache group name</param>
        private CancellationTokenSource GetGroupCancellationToken(string cacheGroup)
        {
            if (cancellationGroups.ContainsKey(cacheGroup))
            {
                return cancellationGroups[cacheGroup];
            }
            return null;
        }

        /// <summary>
        /// Get the cancellation token from the dictionary that matches the group name (or null)
        /// </summary>
        /// <param name="cacheGroup">Cache group name</param>
        private CancellationTokenSource GetOrCreateGroupCancellationToken(string cacheGroup)
        {
            var token = this.GetGroupCancellationToken(cacheGroup);
            if (token == null)
            {
                token = new CancellationTokenSource();
                this.cancellationGroups.AddOrUpdate(cacheGroup, token, (key, oldValue) => token);
            }
            return token;
        }

        /// <summary>
        /// Get an item from the cache, or call a factory function to generate the item and put it in the cache
        /// </summary>
        /// <typeparam name="TItem">Type of item stored</typeparam>
        /// <param name="cacheGroup">Cache Group name that can be used to remove items from the cache</param>
        /// <param name="key">Unique item key</param>
        /// <param name="seconds">Seconds to hold the item in the cache</param>
        /// <param name="factory">Fucntion that creates the item to be cached</param>
        /// <returns>Item from cache or factory</returns>
        public TItem GetOrCreate<TItem>(string cacheGroup, object key, double seconds, Func<ICacheEntry, TItem> factory)
        {
            return this.MemoryCache.GetOrCreate<TItem>(key, (ICacheEntry cacheEntry) =>
            {
                TItem item = factory(cacheEntry);
                cacheEntry.AddExpirationToken(new CancellationChangeToken(this.GetOrCreateGroupCancellationToken(cacheGroup).Token));
                cacheEntry.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(seconds);
                return item;
            }); ;
        }

        /// <summary>
        /// Get an item from the cache, or call a factory function to generate the item and put it in the cache
        /// </summary>
        /// <typeparam name="TItem">Type of item stored</typeparam>
        /// <param name="cacheGroup">Cache Group name that can be used to remove items from the cache</param>
        /// <param name="key">Unique item key</param>
        /// <param name="seconds">Seconds to hold the item in the cache</param>
        /// <param name="factory">Fucntion that creates the item to be cached</param>
        /// <returns>Item from cache or factory</returns>
        public Task<TItem> GetOrCreateAsync<TItem>(string cacheGroup, object key, double seconds, Func<ICacheEntry, Task<TItem>> factory)
        {
            return this.MemoryCache.GetOrCreateAsync<TItem>(key, (ICacheEntry cacheEntry) =>
            {
                Task<TItem> itemTask = factory(cacheEntry);
                cacheEntry.AddExpirationToken(new CancellationChangeToken(this.GetOrCreateGroupCancellationToken(cacheGroup).Token));
                cacheEntry.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(seconds);
                return itemTask;
            }); ;
        }

        /// <summary>
        /// Get an item from the cache
        /// </summary>
        /// <typeparam name="TItem">Type of item</typeparam>
        /// <param name="key">Cache key</param>
        /// <returns>Item from the cache</returns>
        public TItem Get<TItem>(object key)
        {
            return MemoryCache.Get<TItem>(key);
        }

        /// <summary>
        /// Try to get a value from the cache
        /// </summary>
        /// <typeparam name="TItem">Type of item</typeparam>
        /// <param name="key">Unique item key</param>
        /// <param name="value">Output cache item</param>
        /// <returns>True if found</returns>
        public bool TryGetValue<TItem>(object key, out TItem value)
        {
            return this.MemoryCache.TryGetValue(key, out value);
        }

        /// <typeparam name="TItem">Type of item</typeparam>
        /// <param name="cacheGroup">Cache Group name that can be used to remove items from the cache</param>
        /// <param name="key">Unique item key</param>
        /// <param name="seconds">Seconds to hold the item in the cache</param>
        public TItem Set<TItem>(string cacheGroup, object key, TItem value, double seconds)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(seconds)
            };
            options.ExpirationTokens.Add(new CancellationChangeToken(this.GetOrCreateGroupCancellationToken(cacheGroup).Token));
            return this.MemoryCache.Set(key, value, options);
        }
    }
}
