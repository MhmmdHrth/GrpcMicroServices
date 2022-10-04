using Microsoft.EntityFrameworkCore;
using ProductGrpc.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProductGrpc.Data
{
    public sealed class ProductsContextSeed
    {
        public static async Task SeedAsync(ProductsContext context, CancellationToken token = default)
        {
            if (!await context.Products.AnyAsync(token))
            {
                await context.Products.AddRangeAsync(DataSeed, token);
                await context.SaveChangesAsync(token);
            }
        }

        private static IEnumerable<Product> DataSeed => new List<Product>
        {
                new Product
                {
                    ProductId = 1,
                    Name = "Mi10T",
                    Description = "New Xiaomi Phone Mi10T",
                    Price = 699,
                    Status = ProductStatus.INSTOCK,
                    CreatedTime = DateTime.UtcNow
                },

                new Product
                {
                    ProductId = 2,
                    Name = "P40",
                    Description = "New Huawei Phone P40",
                    Price = 899,
                    Status = ProductStatus.INSTOCK,
                    CreatedTime = DateTime.UtcNow
                },

                new Product
                {
                    ProductId = 3,
                    Name = "A50",
                    Description = "New Samsung Phone A50",
                    Price = 399,
                    Status = ProductStatus.INSTOCK,
                    CreatedTime = DateTime.UtcNow
                },
        };
    }
}
