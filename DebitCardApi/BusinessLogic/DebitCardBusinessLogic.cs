using DebitCardApi.DataLayer;

namespace DebitCardApi.BusinessLogic;

public class DebitCardBusinessLogic(IDebitCardDataLayer debitCardDataLayer) : IDebitCardBusinessLogic
{
    public async Task<CreatePaymentReply> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var paymentId = await debitCardDataLayer.CreatePaymentAsync(request);

        return new CreatePaymentReply
        {
            PaymentId = paymentId,
            Message = "Payment created"
        };
    }
}
