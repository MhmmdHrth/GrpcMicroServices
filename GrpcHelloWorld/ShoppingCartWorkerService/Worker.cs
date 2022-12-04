using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductGrpc.Protos;
using ShoppingCartGrpc.Protos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCartWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Connecting To Server");
            Thread.Sleep(2000);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                //Creating Client For SC and Product Service
                using var scChannel = GrpcChannel.ForAddress(_configuration.GetValue<string>("WorkerService:ShoppingCartServerUrl"));
                var scClient = new ShoppingCartProtoService.ShoppingCartProtoServiceClient(scChannel);

                using var productChannel = GrpcChannel.ForAddress(_configuration.GetValue<string>("WorkerService:ProductServerUrl"));
                var productClient = new ProductProtoService.ProductProtoServiceClient(productChannel);

                //Create SC if not exist
                await GetOrCreateShoppingCartAsync(scClient);

                //open sc client stream
                using var streamingShoppingCartItem = scClient.AddItemIntoShoppingCart();

                //Retrieve products from product grpc with server stream
                _logger.LogInformation("GetAllProducts Started...");
                using var products = productClient.GetAllProducts(new GetAllProductRequest());
                await foreach (var product in products.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine($"GetAllProductAsync Response: {product}");

                    //Add sc items into sc with client stream
                    var addNewScItem = new AddItemIntoShoppingCartRequest
                    {
                        Username = _configuration.GetValue<string>("WorkerService:Username"),
                        DiscountCode = "CODE_100",
                        NewCartItem = new ShoppingCartItemModel
                        {
                            ProductId = product.ProductId,
                            ProductName = product.Name,
                            Price = product.Price,
                            Color = "Black",
                            Quantity = 1
                        }
                    };

                    await streamingShoppingCartItem.RequestStream.WriteAsync(addNewScItem);
                    _logger.LogInformation($"ShoppingCart Client Stream Added New Item: {addNewScItem}");
                }

                await streamingShoppingCartItem.RequestStream.CompleteAsync();
                _logger.LogInformation($"AddItemIntoShoppingCart Client Stream Response: {await streamingShoppingCartItem}");

                await Task.Delay(_configuration.GetValue<int>("WorkerService:TaskInterval"), stoppingToken);
            }
        }

        private async Task<ShoppingCartModel> GetOrCreateShoppingCartAsync(ShoppingCartProtoService.ShoppingCartProtoServiceClient scClient)
        {
            //try to get shopping cart
            //create sc
            ShoppingCartModel shoppingCart;
            try
            {
                _logger.LogInformation("Get Shopping Cart Started...");

                shoppingCart = await scClient.GetShoppingCartAsync(new GetShoppingCartRRequest 
                {
                    Username = _configuration.GetValue<string>("WorkerService:Username")
                });

            }
            catch (RpcException exception)
            {
                if (exception.StatusCode.Equals(StatusCode.NotFound))
                {
                    _logger.LogInformation("Create Shopping Cart Started...");

                    shoppingCart = await scClient.CreateShoppingCartAsync(new ShoppingCartModel
                    {
                        Username = _configuration.GetValue<string>("WorkerService:Username")
                    });
                }
                else
                    throw exception;
            }

            _logger.LogInformation($"Shopping Cart Response: {shoppingCart}");
            return shoppingCart;
        }
    }
}
