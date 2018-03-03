using GeekyMonkey.DotNetCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace GeekyMonkey.Example
{
    class Program
    {
        static IServiceProvider serviceProvider;

        static void Main(string[] args)
        {
            // Setup services with dependency injector
            serviceProvider = ConfigureServices();

            // Get an instance of our data provider and log serivce
            var productProvider = serviceProvider.GetService<ProductProvider>();
            var logService = serviceProvider.GetRequiredService<LogService>();

            // Get beer list multiple times - should only generate the data once
            for (int i = 0; i < 3; i++)
            {
                logService.Log("Requesting beer list.");
                var beer = productProvider.GetProducts("Groceries", "Beer");
                logService.Log(beer);
                logService.Log("");
            }

            // Add a new beer to the database
            logService.Log("");
            logService.Log("++ Adding a new Beer ++");
            logService.Log("");
            productProvider.AddProduct("Groceries", "Beer", "Lagunitas IPA");

            // Get beer list multiple times - should only generate the data once
            for (int i = 0; i < 3; i++)
            {
                logService.Log("Requesting beer list.");
                var beer = productProvider.GetProducts("Groceries", "Beer");
                logService.Log(beer);
                logService.Log("");
            }
        }

        /// <summary>
        /// Register services with dependency injector
        /// </summary>
        /// <returns>Service provider</returns>
        private static IServiceProvider ConfigureServices()
        {
            ServiceCollection services = new ServiceCollection();

            // Setup memory cach service
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheService, MemoryCacheService>();
            services.AddTransient<ProductProvider, ProductProvider>();

            // Add a logging service for output
            services.AddSingleton<LogService, LogService>();

            // Done adding services
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
