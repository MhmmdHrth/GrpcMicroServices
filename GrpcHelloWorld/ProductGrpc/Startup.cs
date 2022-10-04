using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductGrpc.Data;
using ProductGrpc.Extensions;
using ProductGrpc.Services;

namespace ProductGrpc
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(opt =>
            {
                opt.EnableDetailedErrors = true;
            });
            services.AddDbContext<ProductsContext>(x => x.UseInMemoryDatabase("Products"));
            services.AddAutoMapper(typeof(Startup).Assembly);
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
                endpoints.MapGrpcService<ProductService>();
            });
        }
    }
}
