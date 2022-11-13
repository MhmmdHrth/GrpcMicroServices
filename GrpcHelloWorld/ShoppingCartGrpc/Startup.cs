using DiscountGrpc.Protos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShoppingCartGrpc.Data;
using ShoppingCartGrpc.Extensions;
using ShoppingCartGrpc.Mapper;
using ShoppingCartGrpc.Services;
using System;

namespace ShoppingCartGrpc
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(x =>
            {
                x.Address = new Uri(Configuration["GrpcConfigs:DiscountUrl"]);
            });

            services.AddScoped<DiscountService>();

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
