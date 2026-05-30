namespace DebitCardApi.DataLayer;

public interface IReservationDataLayer
{
    Task<IReadOnlyCollection<Reservation>> GetReservationsAsync(string cardNumber);

    Task<string> RegisterReservationAsync(Reservation reservation);

    Task SignReservationAsync(string reservationId, string signature);
}
