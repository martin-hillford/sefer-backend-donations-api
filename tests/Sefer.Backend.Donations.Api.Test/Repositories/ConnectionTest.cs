namespace Sefer.Backend.Donations.Api.Test.Repositories;

[TestClass]
public class ConnectionTest
{
    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public async Task DisposeTest()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.AddNpgsqlConfiguration();
        
        var connectionProvider = new DbConnectionProvider(serviceProvider.Object);
        var connection = connectionProvider.GetConnection();
        
        connection.Dispose();

        await connection.ExecuteAsync("select * from users");
    }
    
    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public async Task DisposeAsyncTest()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.AddNpgsqlConfiguration();
        
        var connectionProvider = new DbConnectionProvider(serviceProvider.Object);
        var connection = (Connection)connectionProvider.GetConnection();
        
        await connection.DisposeAsync();

        await connection.QueryAsync<object>("select * from users");
    }
}