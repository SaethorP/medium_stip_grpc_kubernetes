using DebitCardApi.DataLayer;

namespace DebitCardApi.BusinessLogic;

public class ReservationBusinessLogic(IReservationDataLayer reservationDataLayer) : IReservationBusinessLogic
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
            Message = "Reservation registered"
        };
    }

    public async Task<SignReservationReply> SignReservationAsync(SignReservationRequest request)
    {
        await reservationDataLayer.SignReservationAsync(request.ReservationId, request.Signature);

        return new SignReservationReply
        {
            Message = "Reservation signed"
        };
    }
}
