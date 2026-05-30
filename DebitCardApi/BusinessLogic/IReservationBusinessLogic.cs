namespace DebitCardApi.BusinessLogic;

public interface IReservationBusinessLogic
{
    Task<GetReservationsReply> GetReservationsAsync(GetReservationsRequest request);

    Task<RegisterReservationsReply> RegisterReservationsAsync(RegisterReservationsRequest request);

    Task<SignReservationReply> SignReservationAsync(SignReservationRequest request);
}
