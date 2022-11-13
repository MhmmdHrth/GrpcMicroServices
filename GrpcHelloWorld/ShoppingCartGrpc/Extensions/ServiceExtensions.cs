using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ShoppingCartGrpc.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCartGrpc.Extensions
{
    public static class ServiceExtensions
    {
        public static async Task SeedDatabase(this IApplicationBuilder app, CancellationToken cancellationToken = default)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                try
                {
                    Console.WriteLine("Seeding Data");

                    var seed = scope.ServiceProvider.GetRequiredService<ShoppingCartContext>();
                    await ShoppingCartContextSeed.SeedAsync(seed, cancellationToken);

                    Console.WriteLine("Seed Succeeded");
                }
                catch (Exception err)
                {
                    Console.WriteLine($"Seed Failed, {err.Message}");
                }
            }
        }
    }
}
