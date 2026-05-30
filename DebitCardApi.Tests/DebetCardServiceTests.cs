using DebitCardApi;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DebitCardApi.Tests;

public class GreeterServiceTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task ReservationWorkflow_CallsGrpcServiceSuccessfully()
    {
        using var channel = GrpcChannel.ForAddress(factory.Server.BaseAddress, new GrpcChannelOptions
        {
            HttpHandler = factory.Server.CreateHandler()
        });

        var client = new Greeter.GreeterClient(channel);

        var registerReply = await client.RegisterReservationsAsync(new RegisterReservationsRequest
        {
            Reservation = new Reservation
            {
                CardNumber = "4111111111111111",
                Amount = 25.50,
                Merchant = "Test Merchant"
            }
        });

        Assert.False(string.IsNullOrWhiteSpace(registerReply.ReservationId));
        Assert.Equal("Reservation registered", registerReply.Message);

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

        Assert.Empty(reservationsReply.Reservations);
    }
}
