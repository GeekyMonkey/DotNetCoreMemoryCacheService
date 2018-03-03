using GeekyMonkey.DotNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeekyMonkey.Example
{
    /// <summary>
    /// Simulates accessing product data from an expensive to hit database or api
    /// </summary>
    public class ProductProvider
    {
        /// <summary>
        /// Construct the data provider
        /// </summary>
        /// <param name="memoryCacheService">Injected memory cache service</param>
        public ProductProvider(MemoryCacheService memoryCacheService, LogService logService)
        {
            this.LogSerivce = logService;
            this.MemoryCacheService = memoryCacheService;
        }

        /// <summary>
        /// Injected log service
        /// </summary>
        private readonly LogService LogSerivce;

        /// <summary>
        /// Injected memory cache service
        /// </summary>
        private readonly MemoryCacheService MemoryCacheService;

        /// <summary>
        /// Our complete product database
        /// </summary>
        private List<ProductModel> ProductData = new List<ProductModel>
        {
            new ProductModel { ProductCategory = "Groceries", ProductType = "Beer", ProductName = "Guinness"},
            new ProductModel { ProductCategory = "Groceries", ProductType = "Beer", ProductName = "Beamish"},
            new ProductModel { ProductCategory = "Groceries", ProductType = "Fruit", ProductName = "Apple"},
            new ProductModel { ProductCategory = "Groceries", ProductType = "Fruit", ProductName = "Orange"},
            new ProductModel { ProductCategory = "Groceries", ProductType = "Meat", ProductName = "Bacon"},
            new ProductModel { ProductCategory = "Groceries", ProductType = "Meat", ProductName = "Steak"},
            new ProductModel { ProductCategory = "Tools", ProductType = "Hand Tool", ProductName = "Hammer"},
            new ProductModel { ProductCategory = "Tools", ProductType = "Hand Tool", ProductName = "Saw"},
        };

        /// <summary>
        /// Get products for a given category and type
        /// </summary>
        /// <param name="category">Category filter</param>
        /// <param name="productType">Product type filter</param>
        /// <returns></returns>
        public List<ProductModel> GetProducts(string category, string productType)
        {
            var productList = MemoryCacheService.GetOrCreate<List<ProductModel>>(
                $"Products_Category={category}", $"GetProducts_Category={category}_Type={productType}",
                60, (cacheEntry) => {

                    LogSerivce.Log($"!!! Generating List for category={category} type={productType} !!! <--- Expensive!");
                    var filteredProducts = ProductData
                        .Where(p => p.ProductCategory == category && p.ProductType == productType)
                        .OrderBy(p => p.ProductName)
                        .ToList();
                    return filteredProducts;
                });

            return productList;
        }

        /// <summary>
        /// Add a new product to the databaes (and invalidate the cache)
        /// </summary>
        /// <param name="category">new product category</param>
        /// <param name="productType">new product type</param>
        /// <param name="productName">new product name</param>
        public void AddProduct(string category, string productType, string productName)
        {
            ProductData.Add(new ProductModel
            {
                ProductCategory = category,
                ProductType = productType,
                ProductName = productName
            });

            MemoryCacheService.ClearCacheGroup($"Products_Category={category}");
        }
    }
}
