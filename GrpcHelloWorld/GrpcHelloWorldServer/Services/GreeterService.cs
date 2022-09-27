using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GrpcHelloWorldServer
{
    public class GreeterService : HelloService.HelloServiceBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
        {
            var response = new HelloResponse
            {
                Message = $"Hello {request.Name}"
            };

            return Task.FromResult(response);
        }
    }
}
