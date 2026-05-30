using DebitCardApi.BusinessLogic;
using Grpc.Core;

namespace DebitCardApi.Services;

public class ReservationService(
    IReservationBusinessLogic reservationBusinessLogic,
    ILogger<ReservationService> logger) : Greeter.GreeterBase
{
    public override async Task<GetReservationsReply> GetReservations(GetReservationsRequest request, ServerCallContext context)
    {
        logger.LogInformation("Getting reservations for card {CardNumber}", request.CardNumber);

        return await reservationBusinessLogic.GetReservationsAsync(request);
    }

    public override async Task<RegisterReservationsReply> RegisterReservations(RegisterReservationsRequest request, ServerCallContext context)
    {
        logger.LogInformation("Registering reservation for card {CardNumber}", request.Reservation.CardNumber);

        return await reservationBusinessLogic.RegisterReservationsAsync(request);
    }

    public override async Task<SignReservationReply> SignReservation(SignReservationRequest request, ServerCallContext context)
    {
        logger.LogInformation("Signing reservation {ReservationId}", request.ReservationId);

        return await reservationBusinessLogic.SignReservationAsync(request);
    }
}
