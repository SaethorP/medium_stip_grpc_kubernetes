using DebitCardApi.BusinessLogic;
using DebitCardApi.DataLayer;
using DebitCardApi.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(50051, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps();
    });
});

// Add services to the container.
builder.Services.AddGrpc();

var stipEnabled = bool.TryParse(builder.Configuration["STIP_Enabled"], out var enabled) && enabled;
if (stipEnabled)
{
    builder.Services.AddSingleton<IDebitCardDataLayer, StipDebitCardDataLayer>();
    builder.Services.AddScoped<IDebitCardBusinessLogic, StipDebitCardBusinessLogic>();
}
else
{
    builder.Services.AddSingleton<IDebitCardDataLayer, InMemoryDebitCardDataLayer>();
    builder.Services.AddScoped<IDebitCardBusinessLogic, DebitCardBusinessLogic>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<DebitCardGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

public partial class Program;
