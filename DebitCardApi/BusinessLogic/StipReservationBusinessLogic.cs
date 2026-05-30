using DebitCardApi.DataLayer;

namespace DebitCardApi.BusinessLogic;

public class StipReservationBusinessLogic(IReservationDataLayer reservationDataLayer) : IReservationBusinessLogic
{
    public async Task<GetReservationsReply> GetReservationsAsync(GetReservationsRequest request)
    {
        var reservations = await reservationDataLayer.GetReservationsAsync(request.CardNumber);

        var reply = new GetReservationsReply();
        reply.Reservations.AddRange(reservations);

        return reply;
    }

    public async Task<RegisterReservationsReply> RegisterReservationsAsync(RegisterReservationsRequest request)
    {
        var reservationId = await reservationDataLayer.RegisterReservationAsync(request.Reservation);

        return new RegisterReservationsReply
        {
            ReservationId = reservationId,
            Message = "STIP reservation registered"
        };
    }

    public async Task<SignReservationReply> SignReservationAsync(SignReservationRequest request)
    {
        await reservationDataLayer.SignReservationAsync(request.ReservationId, request.Signature);

        return new SignReservationReply
        {
            Message = "STIP reservation signed"
        };
    }
}
