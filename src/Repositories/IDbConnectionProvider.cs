namespace Sefer.Backend.Donations.Api.Repositories;

public interface IDbConnectionProvider
{
    public IConnection GetConnection();
}