# DotNet Core Memory Cache Service

## :poop: Change Log


---
## Version 1.0.1 - 3 March 2018

* Added `RemoveItem(string cacheKey)` function

---
## Version 1.0.0 - 3 March 2018

* `ClearCacheGroup(string groupName)`
* `ClearAll()`
* `TItem GetOrCreate<TItem>(string cacheGroup, object key, double seconds, Func<ICacheEntry, TItem> factory)`
* `Task<TItem> GetOrCreateAsync<TItem>(string cacheGroup, object key, double seconds, Func<ICacheEntry, Task<TItem>> factory)`