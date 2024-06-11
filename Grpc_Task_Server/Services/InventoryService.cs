using Grpc.Core;
using Grpc_Task_Server.Protos;
using System.Reflection.Metadata.Ecma335;
using static Grpc_Task_Server.Protos.InventoryService;

namespace Grpc_Task_Server.Services
{
    public class InventoryService : InventoryServiceBase
    {
        private static readonly List<Product> Products = new()
        {
            new Product { Id = 1, Name = "Laptop", Price = 1500,Category="Accessories" },
            new Product { Id = 2, Name = "Smartphone", Price = 6000 ,Category="Accessories"},
            new Product { Id = 3, Name = "Tablet", Price = 3000 , Category = "Accessories"}
        };

        public override Task<ProductResponse> GetProductById(ProductRequest request, ServerCallContext context)
        {

            var product = Products.Find(p => p.Id == request.Id);

            if (product == null) 
            { 
                return Task.FromResult(new ProductResponse{
                    Exists = false,
                });
            
            }
            else
            {
                return Task.FromResult(new ProductResponse
                {
                    Exists = true,
                });
            }
        }

        public override Task<AddProductResponse> AddProduct(AddProductRequest request, ServerCallContext context)
        {
            Products.Add(request.Product);
            return Task.FromResult(new AddProductResponse { Product = request.Product });
        }

        public override Task<UpdateProductResponse> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
        {
            var index = Products.FindIndex(p => p.Id == request.Product.Id);
            Products[index] = request.Product;

            return Task.FromResult(new UpdateProductResponse {Product = request.Product});


        }

        public override async Task<BulkProductResponse> AddBulkProducts(IAsyncStreamReader<Product> request, ServerCallContext context)
        {
            int count = 0;
            await foreach (var product in request.ReadAllAsync())
            {
                if (!Products.Exists(p => p.Id == product.Id))
                {
                    Products.Add(product);
                    count++;
                }
            }
            return new BulkProductResponse { Count = count };
        }

        public override async Task GetProductReport(ProductReportRequest request, IServerStreamWriter<Product> response, ServerCallContext context)
        {
            var filteredProducts = Products.AsEnumerable();

            if (!string.IsNullOrEmpty(request.Category))
            {
                filteredProducts = filteredProducts.Where(p => p.Category == request.Category);
            }

            if (request.OrderByPrice)
            {
                filteredProducts = filteredProducts.OrderBy(p => p.Price);
            }

            foreach (var product in filteredProducts)
            {
                await response.WriteAsync(product);
            }
        }





    }
}
