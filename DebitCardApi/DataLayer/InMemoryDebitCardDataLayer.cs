namespace DebitCardApi.DataLayer;

public class InMemoryDebitCardDataLayer : IDebitCardDataLayer
{
    public Task<string> CreatePaymentAsync(CreatePaymentRequest request)
    {
        return Task.FromResult(Guid.NewGuid().ToString());
    }
}
