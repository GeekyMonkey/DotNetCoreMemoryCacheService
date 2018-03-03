# DotNet Core Memory Cache Service

## :question:Purpose

The DotNet Core [Microsoft.Extensions.Caching.Memory](https://github.com/aspnet/Caching/tree/dev/src/Microsoft.Extensions.Caching.Memory) lacks the ability to easily remove groups of items from the cache.  This is a [know limitation](https://github.com/aspnet/Caching/issues/187) which apparently there is no interest in addressing.

---
## :point_down: Download

The project is published to [NuGet](https://www.nuget.org/packages/GeekyMonkey.DotNetCoreMemoryCacheService/)

---
## :small_blue_diamond: Compatability

* NET Standard + Platform Extensions,Version=v2.0
* NET Core + Platform Extensions,Version=v2.0

### Dependencies
* Microsoft.Extensions.Caching.Memory (>= 2.0.0)
* Microsoft.Extensions.DependencyInjection (>= 2.0.0)

---
## :heavy_check_mark: Features

* When adding items to the cache, specify the group they belong to
* Remove a group of cached items by group name
* Remove all items in the cache
* Specify the item's cache lifetime in seconds when adding an item to the cache
* Synchronous and Asynchonous factory lamda functions

---
## :alarm_clock:Future
Please feel free to make a pull request. :)
* Implement other methods from IMemoryCache
* Unit tests
* Query the cache status

---
## :beer: Example
Run the included example dotnet console application which demonstrates a simulated database of products where accessing the database is an expensive operation, so these query results are meant to be cached in a product provider.

When a change is made to the product database, the cache has items removed from that product's categories, so when future requests for product lists will pick up the new product immediately.

The console output shows that the first time the beer list is queries, an expensive database operation happens, and subsequent request are served from the cache.

Once a new item is added to the category, the cache for that category becomes invalidated, and the next reqeust for the beer list correctly results in another expensive database hit.

---
## 🔨Getting Started

### Install
Install from [nuget](https://www.nuget.org/packages/GeekyMonkey.DotNetCoreMemoryCacheService/) by running

`dotnet add package GeekyMonkey.DotNetCoreMemoryCacheService --version 1.0.0`

### Register Services
In your application startup, register the following two services with the dependency injection system.

```
    // register memory cach service
    services.AddMemoryCache();
    services.AddSingleton<MemoryCacheService, MemoryCacheService>();
```

The first one is the MemoryCache service included in DotNet Core.
The second is our extension library. These are both singletons or else what's the point. ;)

### Get or Create Cacheable Item
Probably best to do this sort of thing inside of a data provider class, but that's not a hard requirement.

With your instance of the MemoryCacheService call `GetOrCreate<MyItemType>()`.  The parameters are
* **string** Cache Group (used for later removing groups of items)
* **string** Unique cache key
* **double** Seconds before the item automatically is removed from the cache
* **(cacheEntry)=>MyItemType** A factory functon that will generate or retrieve your value if it is not in the cache. This is where you find the data in a databaes, or call an API to download the data.

*Example:*
```
MemoryCacheService.GetOrCreate<List<ProductModel>>(
    $"MyCacheGroup", $"MyCacheKey",
    60, (cacheEntry) => {
        // Not in the cache - get from the database
        return DownloadMyProductData();
    });
```

### Async
There is also a `GetOrCreateAsync<>()` function that works exactly as `GetOrCreate<>()` except that it runs asyncrounsly and needs to be awaited. *Use this wherever possible*.

### Removing Items from Cache
When a change is saved to the database, and you want your users to see this new data, use one of the below functions for the different ranges of item(s) to be deleted from cache. Note that these are not refreshed back into the cach until the next time they are requested.

### Removing a Single Item from the cache
Boring! You could already do this. But maybe this is your thing. I'm not going to judge. The `RemoveItem(string cacheKey)` is there for this sort of thing.

### Removing a Group of Items from the cache
When you call `ClearCacheGroup(string groupName)`, any item still in the cache that's registered with that group will be removed immediately.

### Removing everything
The nuclear option. There is a `ClearAll()` that removes all of the cache groups.

---
## 💥 Warnings 
If deploying to a server farm and you want to stick with in-memory cache, be sure to configure the load balancer to use sticky sessions. Otherwise you should use a distributed cache.

---
## :raising_hand: Contact

* Got a problem or suggestion? Use the [Issues](https://github.com/GeekyMonkey/DotNetCoreMemoryCacheService/issues) feature to let me know.
* Feel like your own code changes would be helpful? Issue a [Pull Request](https://github.com/GeekyMonkey/DotNetCoreMemoryCacheService/pulls).
* Did I save you some time? Feeling thankfull? Go on, [buy me a beer. :beer:](http://geekymonkey.azurewebsites.net/Home/Contact)
<form action="https://www.paypal.com/cgi-bin/webscr" method="post" target="_top" style="margin-left:30px">
<input type="hidden" name="cmd" value="_s-xclick">
<input type="hidden" name="hosted_button_id" value="LB9723JHYDXV4">
<input type="image" src="https://www.paypalobjects.com/en_US/i/btn/btn_donate_SM.gif" border="0" name="submit" alt="PayPal - The safer, easier way to pay online!">
<img alt="" border="0" src="https://www.paypalobjects.com/en_US/i/scr/pixel.gif" width="1" height="1">
</form>
