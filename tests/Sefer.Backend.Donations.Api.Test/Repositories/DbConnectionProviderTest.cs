namespace Sefer.Backend.Donations.Api.Test.Repositories;

[TestClass]
public class DbConnectionProviderTest
{
    [TestMethod]
    public void GetConnection()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.AddNpgsqlConfiguration();
        
        var provider = new DbConnectionProvider(serviceProvider.Object);
        var connection = provider.GetConnection();
        
        Assert.IsNotNull(connection);
    }
}