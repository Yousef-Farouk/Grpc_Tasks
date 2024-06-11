using Grpc.Net.Client;
using static Grpc_Task_Client.Protos.InventoryService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the gRPC client
builder.Services.AddSingleton(sp =>
{
    var channel = GrpcChannel.ForAddress("https://localhost:7062");
    return new InventoryServiceClient(channel);
});


//builder.Services.AddGrpc();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

