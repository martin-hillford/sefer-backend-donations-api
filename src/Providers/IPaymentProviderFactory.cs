namespace Sefer.Backend.Donations.Api.Providers;

public interface IPaymentProviderFactory
{
    public IPaymentProvider? Create(string? providerName);
}