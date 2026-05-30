namespace DebitCardApi.DataLayer;

public class StipDebitCardDataLayer : IDebitCardDataLayer
{
    public Task<string> CreatePaymentAsync(CreatePaymentRequest request)
    {
        return Task.FromResult($"STIP-{Guid.NewGuid()}");
    }
}
