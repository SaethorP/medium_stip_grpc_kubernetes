namespace DebitCardApi.DataLayer;

public interface IDebitCardDataLayer
{
    Task<string> CreatePaymentAsync(CreatePaymentRequest request);
}
