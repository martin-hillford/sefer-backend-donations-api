namespace Sefer.Backend.Donations.Api.Test;

public static class MoqExtensions
{
    internal static IDbConnectionProvider GetProvider(this Mock<IConnection> connection)
    {
        var connectionProvider = new Mock<IDbConnectionProvider>();
        connectionProvider.Setup(x => x.GetConnection()).Returns(connection.Object);
        return connectionProvider.Object;
    }

    internal static Mock<IServiceProvider> AddNpgsqlConfiguration(this Mock<IServiceProvider> provider)
    {
        var value = new Mock<IConfigurationSection>();
        value.Setup(v => v.Value).Returns("Server=127.0.0.1;Port=5432;Database=db;User Id=user;Password=password;");
        
        var section = new Mock<IConfigurationSection>();
        section.Setup(s => s.GetSection("ConnectionString")).Returns(value.Object);
        
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c.GetSection("Database")).Returns(section.Object);
        
        provider.Setup(x => x.GetService(typeof(IConfiguration))).Returns(configuration.Object);
        return provider;
    }
}