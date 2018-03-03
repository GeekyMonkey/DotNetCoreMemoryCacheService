# DotNet Core Memory Cache Service

## Purpose

The DotNet Core [Microsoft.Extensions.Caching.Memory](https://github.com/aspnet/Caching/tree/dev/src/Microsoft.Extensions.Caching.Memory) lacks the ability to easily remove groups of items from the cache.  This is a [know limitation](https://github.com/aspnet/Caching/issues/187) which apparently there is no interest in addressing.

## Compatability

* NET Standard + Platform Extensions,Version=v2.0
* NET Core + Platform Extensions,Version=v2.0

## Features

* When adding items to the cache, specify the group they belong to
* Remove a group of cached items by group name
* Remove all items in the cache
* Specify the item's cache lifetime in seconds when adding an item to the cache
* Synchronous and Asynchonous factory lamda functions

## Future
Please feel free to make a pull request. :)
* Implement other methods from IMemoryCache
* Unit tests
* Query the cache status

## Example
Run the included example dotnet console application which demonstrates a simulated database of products where accessing the database is an expensive operation, so these query results are meant to be cached in a product provider.

When a change is made to the product database, the cache has items removed from that product's categories, so when future requests for product lists will pick up the new product immediately.

The console output shows that the first time the beer list is queries, an expensive database operation happens, and subsequent request are served from the cache.

Once a new item is added to the category, the cache for that category becomes invalidated, and the next reqeust for the beer list correctly results in another expensive database hit.

## Getting Started

If deploying to a server farm and you want to stick with in-memory cache, be sure to configure the load balancer to use sticky sessions.
