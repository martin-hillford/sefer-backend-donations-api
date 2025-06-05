namespace Sefer.Backend.Donations.Api.Providers;

public abstract class PaymentProvider(IServiceProvider provider) : IPaymentProvider
{
    private readonly INetworkProvider? _networkProvider = provider.GetService<INetworkProvider>();
    
    protected async Task<ISite?> GetSite(string siteName)
    {
        if (_networkProvider == null) return null;
        var sites = await _networkProvider.GetSitesAsync();
        return sites.FirstOrDefault(s => s.Name == siteName || s.Hostname == siteName);
    }

    public abstract Task<DonationResponse?> CreateResponseAsync(string donationId, DonationPromise promise);

    public abstract Task<PaymentStatus?> GetStatus(string paymentId);
}