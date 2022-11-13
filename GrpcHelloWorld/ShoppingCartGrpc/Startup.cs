using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShoppingCartGrpc.Data;
using ShoppingCartGrpc.Extensions;
using ShoppingCartGrpc.Mapper;
using ShoppingCartGrpc.Services;

namespace ShoppingCartGrpc
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddDbContext<ShoppingCartContext>(x => x.UseInMemoryDatabase("ShoppingCart"));
            services.AddAutoMapper(typeof(ShoppingCartProfile).Assembly);
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            await app.SeedDatabase();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ShoppingCartService>();
            });
        }
    }
}
