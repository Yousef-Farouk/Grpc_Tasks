using Grpc.Net.Client;
using Grpc_Task_Client2.Protos;
using static Grpc_Task_Client2.Protos.InventoryService;

namespace Grpc_Task_Client2
{
    internal class Program
    {
        static  void Main(string[] args)
        {
            var response = RequestProduct();
            Console.WriteLine($"Response: {response}");
        }


        public static bool RequestProduct()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7062");
            var client = new InventoryServiceClient(channel);

            var request = new ProductRequest
            {
                Id = 5,
            };

            var response =  client.GetProductById(request);

            return response.Exists;
        }
    }
}
