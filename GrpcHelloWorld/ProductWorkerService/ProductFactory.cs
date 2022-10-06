using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductGrpc.Protos;
using System;
using System.Threading.Tasks;

namespace ProductWorkerService
{
    public class ProductFactory
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public ProductFactory(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Task<AddProductRequest> Generate()
        {
            var productStatusLength = System.Enum.GetValues(typeof(ProductStatus)).Length - 1;
            var time = DateTime.UtcNow;

            return Task.FromResult(new AddProductRequest
            {
                Product = new ProductModel
                {
                    Name = $"New Product {Guid.NewGuid()}_{time.Date}",
                    Description = $"New Description_{Guid.NewGuid()}_{time.Date}",
                    Price = new Random().Next(200, 7999),
                    Status = (ProductStatus)new Random().Next(0, productStatusLength),
                    CreatedTime = Timestamp.FromDateTime(time)
                }
            });
        }
    }
}
