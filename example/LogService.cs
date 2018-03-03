using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeekyMonkey.Example
{
    /// <summary>
    /// Simple logging service for output
    /// </summary>
    public class LogService
    {
        /// <summary>
        /// Output text
        /// </summary>
        /// <param name="message">Message to output</param>
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Output a list of products
        /// </summary>
        /// <param name="products">List of products</param>
        public void Log(ProductModel product)
        {
            this.Log($" Product={product.ProductName}");
        }

        /// <summary>
        /// Output a list of products
        /// </summary>
        /// <param name="products">List of products</param>
        public void Log(List<ProductModel> products)
        {
            List<string> productNames = new List<string>();
            foreach (var product in products)
            {
                productNames.Add(product.ProductName);
            }
            this.Log($"{products.Count} products: {string.Join(", ", productNames)}");
        }
    }
}
