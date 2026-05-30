using DebitCardApi;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DebitCardApi.Tests;

[Collection("Reservation API")]
public class ReservationServiceTests
{
    [Fact]
    public async Task ReservationWorkflow_UsesDefaultImplementation()
    {
        using var environment = new EnvironmentVariableScope("STIP_Enabled", null);
        using var factory = new WebApplicationFactory<Program>();
        var client = CreateClient(factory);
        var registerReply = await RegisterReservationAsync(client);

        Assert.False(string.IsNullOrWhiteSpace(registerReply.ReservationId));
        Assert.Equal("Reservation registered", registerReply.Message);
        Assert.False(registerReply.ReservationId.StartsWith("STIP-", StringComparison.Ordinal));

        var signReply = await client.SignReservationAsync(new SignReservationRequest
        {
            ReservationId = registerReply.ReservationId,
            Signature = "test-signature"
        });

        Assert.Equal("Reservation signed", signReply.Message);

        var reservationsReply = await client.GetReservationsAsync(new GetReservationsRequest
        {
            CardNumber = "4111111111111111"
        });

        var reservation = Assert.Single(reservationsReply.Reservations);
        Assert.Equal(registerReply.ReservationId, reservation.ReservationId);
    }

    [Fact]
    public async Task ReservationWorkflow_UsesStipImplementation()
    {
        using var environment = new EnvironmentVariableScope("STIP_Enabled", "true");
        using var factory = new WebApplicationFactory<Program>();
        var client = CreateClient(factory);
        var registerReply = await RegisterReservationAsync(client);

        Assert.StartsWith("STIP-", registerReply.ReservationId);
        Assert.Equal("STIP reservation registered", registerReply.Message);

        var signReply = await client.SignReservationAsync(new SignReservationRequest
        {
            ReservationId = registerReply.ReservationId,
            Signature = "test-signature"
        });

        Assert.Equal("STIP reservation signed", signReply.Message);

        var reservationsReply = await client.GetReservationsAsync(new GetReservationsRequest
        {
            CardNumber = "4111111111111111"
        });

        var reservation = Assert.Single(reservationsReply.Reservations);
        Assert.Equal(registerReply.ReservationId, reservation.ReservationId);
    }

    private static Greeter.GreeterClient CreateClient(WebApplicationFactory<Program> webApplicationFactory)
    {
        var channel = GrpcChannel.ForAddress(webApplicationFactory.Server.BaseAddress, new GrpcChannelOptions
        {
            HttpHandler = webApplicationFactory.Server.CreateHandler()
        });

        return new Greeter.GreeterClient(channel);
    }

    private static async Task<RegisterReservationsReply> RegisterReservationAsync(Greeter.GreeterClient client)
    {
        return await client.RegisterReservationsAsync(new RegisterReservationsRequest
        {
            Reservation = new Reservation
            {
                CardNumber = "4111111111111111",
                Amount = 25.50,
                Merchant = "Test Merchant"
            }
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

[CollectionDefinition("Reservation API", DisableParallelization = true)]
public class ReservationApiCollection;
