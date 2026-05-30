using DebitCardApi.BusinessLogic;
using Grpc.Core;

namespace DebitCardApi.Services;

public class DebitCardGrpcService(
    IDebitCardBusinessLogic debitCardBusinessLogic,
    ILogger<DebitCardGrpcService> logger) : DebitCardService.DebitCardServiceBase
{
    public override async Task<CreatePaymentReply> CreatePayment(CreatePaymentRequest request, ServerCallContext context)
    {
        logger.LogInformation("Creating payment for card {CardNumber}", request.CardNumber);

        return await debitCardBusinessLogic.CreatePaymentAsync(request);
    }
}
