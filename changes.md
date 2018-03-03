# DotNet Core Memory Cache Service

## :poop: Change Log


---
## Not yet published

* Added `RemoveItem(string cacheKey)` function

---
## Version 1.0.0

* `ClearCacheGroup(string groupName)`
* `ClearAll()`
* `TItem GetOrCreate<TItem>(string cacheGroup, object key, double seconds, Func<ICacheEntry, TItem> factory)`
* `Task<TItem> GetOrCreateAsync<TItem>(string cacheGroup, object key, double seconds, Func<ICacheEntry, Task<TItem>> factory)`