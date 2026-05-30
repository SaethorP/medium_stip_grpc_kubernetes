namespace DebitCardApi.BusinessLogic;

public interface IDebitCardBusinessLogic
{
    Task<CreatePaymentReply> CreatePaymentAsync(CreatePaymentRequest request);
}
