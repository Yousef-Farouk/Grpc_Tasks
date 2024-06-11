using Grpc.Core;
using Grpc.Net.Client;
using Grpc_Task_Client.Protos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Grpc_Task_Client.Protos.InventoryService;

namespace Grpc_Task_Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryServiceClient _client;

        public InventoryController(InventoryServiceClient client)
        {
            //var channel = GrpcChannel.ForAddress("http://localhost:7062");

            _client = client;
        }


        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            var getResponse = await _client.GetProductByIdAsync(new ProductRequest { Id = product.Id });

            if (getResponse.Exists)
            {
              
                var updateResponse = await _client.UpdateProductAsync(new UpdateProductRequest { Product = product });
                return Ok(new { action = "Product updated", success = updateResponse.Product });
            }
            else
            {
                var addResponse = await _client.AddProductAsync(new AddProductRequest { Product = product });
                return Ok(new { action = "Product added", success = addResponse.Product  });
            }
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> AddProducts([FromBody] IEnumerable<Product> products)
        {
            using var call = _client.AddBulkProducts();

            foreach (var product in products)
            {
                await call.RequestStream.WriteAsync(product);
            }

            await call.RequestStream.CompleteAsync();
            var response = await call.ResponseAsync;

            return Ok($" added {response.Count} products.");
        }


        [HttpGet("GenerateProductReport")]
        public async Task<IActionResult> GenerateProductReport([FromQuery] string category, [FromQuery] bool orderByPrice)
        {
            var request = new ProductReportRequest
            {
                Category = category,
                OrderByPrice = orderByPrice
            };

            using var call = _client.GetProductReport(request);
            var products = new List<Product>();

            await foreach (var product in call.ResponseStream.ReadAllAsync())
            {
                products.Add(product);
            }

            return Ok(products);
        }






    }
}
