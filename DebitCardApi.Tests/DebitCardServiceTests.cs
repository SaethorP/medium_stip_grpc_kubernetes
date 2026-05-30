using DebitCardApi;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DebitCardApi.Tests;

[Collection("Debit Card API")]
public class DebitCardServiceTests
{
    [Fact]
    public async Task CreatePayment_UsesDefaultImplementation()
    {
        using var environment = new EnvironmentVariableScope("STIP_Enabled", null);
        using var factory = new WebApplicationFactory<Program>();
        var client = CreateClient(factory);
        var reply = await CreatePaymentAsync(client);

        Assert.False(string.IsNullOrWhiteSpace(reply.PaymentId));
        Assert.Equal("Payment created", reply.Message);
        Assert.False(reply.PaymentId.StartsWith("STIP-", StringComparison.Ordinal));
    }

    [Fact]
    public async Task CreatePayment_UsesStipImplementation()
    {
        using var environment = new EnvironmentVariableScope("STIP_Enabled", "true");
        using var factory = new WebApplicationFactory<Program>();
        var client = CreateClient(factory);
        var reply = await CreatePaymentAsync(client);

        Assert.StartsWith("STIP-", reply.PaymentId);
        Assert.Equal("STIP payment created", reply.Message);
    }

    private static DebitCardService.DebitCardServiceClient CreateClient(WebApplicationFactory<Program> webApplicationFactory)
    {
        var channel = GrpcChannel.ForAddress(webApplicationFactory.Server.BaseAddress, new GrpcChannelOptions
        {
            HttpHandler = webApplicationFactory.Server.CreateHandler()
        });

        return new DebitCardService.DebitCardServiceClient(channel);
    }

    private static async Task<CreatePaymentReply> CreatePaymentAsync(DebitCardService.DebitCardServiceClient client)
    {
        return await client.CreatePaymentAsync(new CreatePaymentRequest
        {
            CardNumber = "4111111111111111",
            Amount = 25.50,
            Merchant = "Test Merchant"
        });
    }

    private sealed class EnvironmentVariableScope : IDisposable
    {
        private readonly string name;
        private readonly string? originalValue;

        public EnvironmentVariableScope(string name, string? value)
        {
            this.name = name;
            originalValue = Environment.GetEnvironmentVariable(name);
            Environment.SetEnvironmentVariable(name, value);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable(name, originalValue);
        }
    }
}

[CollectionDefinition("Debit Card API", DisableParallelization = true)]
public class DebitCardApiCollection;
