using Microsoft.EntityFrameworkCore;
using ShoppingCartGrpc.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCartGrpc.Data
{
    public class ShoppingCartContextSeed
    {
        public static async Task SeedAsync(ShoppingCartContext context, CancellationToken token = default)
        {
            if (!await context.ShoppingCarts.AnyAsync(token))
            {
                await context.ShoppingCarts.AddRangeAsync(DataSeed, token);
                await context.SaveChangesAsync(token);
            }
        }

        private static IEnumerable<ShoppingCart> DataSeed => new List<ShoppingCart>
        {
                new ShoppingCart
                {
                    UserName = "swn",
                    Items = new List<ShoppingCartItem>
                    {
                        new ShoppingCartItem
                        {
                            Quantity = 2,
                            Color = "Black",
                            Price = 699,
                            ProductId = 1,
                            ProductName = "Mi10T"
                        },
                        new ShoppingCartItem
                        {
                            Quantity = 3,
                            Color = "Black",
                            Price = 699,
                            ProductId = 2,
                            ProductName = "P40"
                        }
                    }
                },
        };
    }
}
