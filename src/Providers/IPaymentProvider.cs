namespace Sefer.Backend.Donations.Api.Providers;

public interface IPaymentProvider
{
    public Task<DonationResponse?> CreateResponseAsync(string donationId, DonationPromise promise);
    
    public Task<PaymentStatus?> GetStatus(string paymentId);
}