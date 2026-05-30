using System.Collections.Concurrent;

namespace DebitCardApi.DataLayer;

public class InMemoryReservationDataLayer : IReservationDataLayer
{
    private readonly ConcurrentDictionary<string, Reservation> reservations = new();

    public Task<IReadOnlyCollection<Reservation>> GetReservationsAsync(string cardNumber)
    {
        var matches = reservations.Values
            .Where(reservation => reservation.CardNumber == cardNumber)
            .Select(reservation => reservation.Clone())
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<Reservation>>(matches);
    }

    public Task<string> RegisterReservationAsync(Reservation reservation)
    {
        var reservationId = Guid.NewGuid().ToString();
        var storedReservation = reservation.Clone();
        storedReservation.ReservationId = reservationId;
        reservations[reservationId] = storedReservation;

        return Task.FromResult(reservationId);
    }

    public Task SignReservationAsync(string reservationId, string signature)
    {
        return Task.CompletedTask;
    }
}
