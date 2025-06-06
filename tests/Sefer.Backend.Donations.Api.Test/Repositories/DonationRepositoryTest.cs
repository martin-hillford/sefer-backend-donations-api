namespace Sefer.Backend.Donations.Api.Test.Repositories;

[TestClass]
public class DonationRepositoryTest
{
    [TestMethod]
    public async Task InsertAsync_Exception()
    {
        var connectionProvider = new Mock<IDbConnectionProvider>();
        var repository = new DonationRepository(connectionProvider.Object);
        var result = await repository.InsertAsync(new Donation());
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task InsertAsync_Success()
    {
        var connection = new Mock<IConnection>();
        connection.Setup(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object?>()));
        var repository = new DonationRepository(connection.GetProvider());        

        var result = await repository.InsertAsync(new Donation());
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task GetByIdAsync_Null()
    {
        var connection = new Mock<IConnection>();
        connection
            .Setup(c => c.QueryAsync<Donation>("select * from donations where id = @id", It.IsAny<object?>()))
            .ReturnsAsync([]);
        var repository = new DonationRepository(connection.GetProvider());
        
        var donation = await repository.GetById(Guid.Empty);
        Assert.IsNull(donation);
    }
    
    [TestMethod]
    public async Task GetByIdAsync_Success()
    {
        var donations = new List<Donation> { new() { Id = "mocked-id" }};
        var connection = new Mock<IConnection>();
        connection
            .Setup(c => c.QueryAsync<Donation>("select * from donations where id = @id", It.IsAny<object?>()))
            .ReturnsAsync(donations);
        var repository = new DonationRepository(connection.GetProvider());
        
        var donation = await repository.GetById(Guid.Empty);
        Assert.IsNotNull(donation);
        Assert.AreEqual(donation.Id, donations.First().Id);
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task GetByIdAsync_Exception()
    {
        var donations = new List<Donation> { new() { Id = "mocked-id-2" }, new() { Id = "mocked-id-2" }};
        var connection = new Mock<IConnection>();
        connection
            .Setup(c => c.QueryAsync<Donation>("select * from donations where id = @id", It.IsAny<object?>()))
            .ReturnsAsync(donations);
        var repository = new DonationRepository(connection.GetProvider());
        
        await repository.GetById(Guid.Empty);
    }

    [TestMethod]
    public async Task UpdateStatusAsync_NoDonation()
    {
        var connectionProvider = new Mock<IDbConnectionProvider>();
        var repository = new DonationRepository(connectionProvider.Object);
        var result = await repository.UpdateStatus(null,null);
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public async Task UpdateStatusAsync_NoStatus()
    {
        var connectionProvider = new Mock<IDbConnectionProvider>();
        var repository = new DonationRepository(connectionProvider.Object);
        var result = await repository.UpdateStatus(new Donation(),null);
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public async Task UpdateStatusAsync_Success()
    {
        // Note: it would be nice to check in this method that the id is indeed used
        var connection = new Mock<IConnection>();
        connection.Setup(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object?>()));
        var repository = new DonationRepository(connection.GetProvider());        

        var result = await repository.UpdateStatus(new Donation { Id = "mocked-id" }, PaymentStatus.Paid);
        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public async Task UpdateStatusAsync_Exception()
    {
        // Note: it would be nice to check in this method that the id is indeed used
        var provider = new Mock<IDbConnectionProvider>();
        var repository = new DonationRepository(provider.Object);        

        var result = await repository.UpdateStatus(new Donation { Id = "mocked-id" }, PaymentStatus.Paid);
        Assert.IsFalse(result);
    }
}