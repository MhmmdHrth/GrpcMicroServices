using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace GrpcHelloWorldClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string serverAddress = "https://localhost:5001";

            using (var channel = GrpcChannel.ForAddress(serverAddress))
            {
                var client = new HelloService.HelloServiceClient(channel);
                var reply = await client.SayHelloAsync(new HelloRequest { Name = "Muhammad Harith" });

                Console.WriteLine($"Greetings: {reply.Message}");
                Console.ReadLine();
            }
        }
    }
}
