namespace Sefer.Backend.Donations.Api.Repositories;

public interface IDonationRepository
{
    public Task<bool> InsertAsync(Donation donation);

    public Task<Donation?> GetById(Guid donationId);

    public Task<bool> UpdateStatus(Donation? donation, PaymentStatus? status);
}