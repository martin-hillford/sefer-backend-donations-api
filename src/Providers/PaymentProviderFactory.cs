namespace Sefer.Backend.Donations.Api.Providers;

public class PaymentProviderFactory(IServiceProvider serviceProvider) : IPaymentProviderFactory
{
    public IPaymentProvider? Create(string? providerName)
    {
        return providerName?.ToLower() switch
        {
            "mollie" => new IdealMollieProvider(serviceProvider),
            _ => null
        };
    }
}