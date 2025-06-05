namespace Sefer.Backend.Donations.Api.Repositories;

public class DonationRepository(IServiceProvider serviceProvider) : IDonationRepository
{
    public async Task<bool> Insert(Donation donation)
    {
        try
        {
            var param = new
            {
                id = donation.Id, creationDate = DateTime.UtcNow, status = (byte)donation.Status,
                amount = donation.Amount, currency = donation.Currency
            };
            const string query = "insert into donations (id, creation_date, status, amount, payment_id, provider, currency) " + 
                                 "values (@id, @creationDate @status, @amount, @paymentId, @provider, @currency )";
            await GetConnection().ExecuteAsync(query, param);
            return true;
        }
        catch(Exception) { return false; }
    }

    public Task<Donation?> GetById(Guid donationId) => GetById(donationId.ToString());
    
    private async Task<Donation?> GetById(string id)
    {
        var connection = GetConnection();
        var donations = await connection.QueryAsync<Donation>($"select * from donations where id = @id", new { id  });
        return donations.FirstOrDefault();
    }

    public async Task<bool> UpdateStatus(Donation? donation, PaymentStatus? status)
    {
        const string query = "update donations set status = @status where id = @id";
        if (donation == null || status == null) return false;
        try
        {
            await GetConnection().ExecuteAsync(query, new { id = donation.Id, status = (byte)status });
            return true;
        }
        catch(Exception) { return false; }
    }
    
    private NpgsqlConnection GetConnection()
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        var connectionString = configuration.GetSection("Database").GetValue<string>("ConnectionString");
        return new NpgsqlConnection(connectionString);
    }
}