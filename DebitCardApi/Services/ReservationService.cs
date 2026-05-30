using Grpc.Core;

namespace DebitCardApi.Services;

public class ReservationService(ILogger<ReservationService> logger) : Greeter.GreeterBase
{
    public override Task<GetReservationsReply> GetReservations(GetReservationsRequest request, ServerCallContext context)
    {
        logger.LogInformation("Getting reservations for card {CardNumber}", request.CardNumber);

        return Task.FromResult(new GetReservationsReply());
    }

    public override Task<RegisterReservationsReply> RegisterReservations(RegisterReservationsRequest request, ServerCallContext context)
    {
        logger.LogInformation("Registering reservation for card {CardNumber}", request.Reservation.CardNumber);

        return Task.FromResult(new RegisterReservationsReply
        {
            ReservationId = Guid.NewGuid().ToString(),
            Message = "Reservation registered"
        });
    }

    public override Task<SignReservationReply> SignReservation(SignReservationRequest request, ServerCallContext context)
    {
        logger.LogInformation("Signing reservation {ReservationId}", request.ReservationId);

        return Task.FromResult(new SignReservationReply
        {
            Message = "Reservation signed"
        });
    }
}
