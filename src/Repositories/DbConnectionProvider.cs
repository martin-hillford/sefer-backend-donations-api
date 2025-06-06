namespace Sefer.Backend.Donations.Api.Repositories;

public class DbConnectionProvider(IServiceProvider serviceProvider) : IDbConnectionProvider
{
    public IConnection GetConnection()
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        var connectionString = configuration.GetSection("Database").GetValue<string>("ConnectionString");
        var connection = new NpgsqlConnection(connectionString);
        return new Connection(connection);
    }
}

