using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using ProductGrpc.Protos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProductGrpcClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Connecting To Server");
            Thread.Sleep(2000);

            var serverAddress = "https://localhost:5001";
            using var channel = GrpcChannel.ForAddress(serverAddress);
            var client = new ProductProtoService.ProductProtoServiceClient(channel);

            Console.WriteLine("Connected To The Server");

            await AddProductAsync(client);
            Console.WriteLine();
            await UpdateProductAsync(client);
            Console.WriteLine();
            await GetProductAsync(client);
            Console.WriteLine();
            await DeleteProductAsync(client);
            Console.WriteLine();
            await InsertBulkProductAsync(client);
            Console.WriteLine();
            await GetAllProductAsync(client);

            Console.ReadKey();
        }

        public static async Task GetProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("GetProductAsync Started...");

            var product = await client.GetProductAsync(new GetProductRequest { ProductId = 4 });

            Console.WriteLine($"GetProductAsync Response: {product}");
        }

        public static async Task GetAllProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            //--------------------------------------------------- GetAllProductAsync ---------------------------------------------------//
            //Console.WriteLine("GetAllProductAsync Started...");
            //using (var products = client.GetAllProducts(new GetAllProductRequest()))
            //{
            //    while (await products.ResponseStream.MoveNext(new CancellationToken())) //GetAllProducts start trigger from here
            //    {
            //        var currentProduct = products.ResponseStream.Current;
            //        Console.WriteLine($"GetAllProductAsync Response: {currentProduct}");
            //    }
            //}

            Console.WriteLine("GetAllProductAsync with C# 9 Started...");

            using var products = client.GetAllProducts(new GetAllProductRequest());
            await foreach (var product in products.ResponseStream.ReadAllAsync()) //GetAllProducts start trigger from here
                Console.WriteLine($"GetAllProductAsync Response: {product}");
        }

        private static async Task AddProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("Add ProductAsync Started...");

            var product = await client.AddProductAsync(new AddProductRequest
            {
                Product = new ProductModel
                {
                    Name = "Iphone 11 Pro Max",
                    Description = "The Power Of Dreams",
                    Price = 1199,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            });

            Console.WriteLine($"Add Product Response: {product}");
        }

        private static async Task UpdateProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("Update ProductAsync Started...");

            var product = await client.UpdateProductAsync(new UpdateProductRequest
            {
                Product = new ProductModel
                {
                    ProductId = 4,
                    Name = "Iphone 11 Pro Max",
                    Description = "Title Changed",
                    Price = 1199,
                    Status = ProductStatus.Low,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            });

            Console.WriteLine($"Update Product Response: {product}");
        }

        private static async Task DeleteProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("Delete ProductAsync Started...");

            var isSuccess = await client.DeleteProductAsync(new DeleteProductRequest { ProductId = 4 });

            Console.WriteLine(value: $"Delete Product Response: {isSuccess.Success}");
        }

        private static async Task InsertBulkProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            Console.WriteLine("Insert Bulk ProductAsync Started...");

            var insertBulk = client.InsertBulkProduct();

            for(int i = 0; i < 3; i++)
            {
                var productModel = new ProductModel
                {
                    Name = $"Handphone_{i}",
                    Description = $"Description_{i}",
                    Price = 1199,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
                };

                await insertBulk.RequestStream.WriteAsync(productModel);
            }

            await insertBulk.RequestStream.CompleteAsync();

            var response = await insertBulk.ResponseAsync;
            Console.WriteLine($"Success: {response.Success}, Insert Count: {response.InsertCount}");
        }
    }
}
