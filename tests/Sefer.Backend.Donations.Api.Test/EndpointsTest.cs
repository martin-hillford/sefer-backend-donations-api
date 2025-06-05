namespace Sefer.Backend.Donations.Api.Test;

[TestClass]
public class EndpointsTest
{
    [TestMethod]
    public async Task CreateDonation_NoProvider()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var promise = new DonationPromise { Provider = "mollie", Amount = 10, Currency = "EUR", Site = "example.com" };
        
        var result = await Endpoints.CreateDonation(serviceProvider.Object, promise);
        
        Assert.IsInstanceOfType<BadRequest>(result);
    }
    
    [TestMethod]
    public async Task CreateDonation_NoRepository()
    {
        var paymentProvider = new Mock<IPaymentProvider>();
        var serviceProvider = GetServiceProvider(paymentProvider);
        var promise = new DonationPromise { Provider = "mollie", Amount = 10, Currency = "EUR", Site = "example.com" };
        
        var result = await Endpoints.CreateDonation(serviceProvider.Object, promise);
        
        Assert.IsInstanceOfType<StatusCodeHttpResult>(result);
    }
    
    [TestMethod]
    public async Task CreateDonation_NoPaymentResponse()
    {
        var paymentProvider = new Mock<IPaymentProvider>();
        var repository = new Mock<IDonationRepository>();
        var serviceProvider = GetServiceProvider(paymentProvider, repository);
        var promise = new DonationPromise { Provider = "mollie", Amount = 10, Currency = "EUR", Site = "example.com" };
        
        var result = await Endpoints.CreateDonation(serviceProvider.Object, promise);
        
        Assert.IsInstanceOfType<BadRequest>(result);
    }
    
    [TestMethod]
    public async Task CreateDonation_NotInserted()
    {
        var paymentProvider = new Mock<IPaymentProvider>();
        paymentProvider
            .Setup(s => s.CreateResponseAsync(It.IsAny<string>(), It.IsAny<DonationPromise>()))
            .ReturnsAsync(new DonationResponse { PaymentId = "payment-id", Method = Method.Get, Url = "example.com", Data = paymentProvider });
        
        var repository = new Mock<IDonationRepository>();
        var serviceProvider = GetServiceProvider(paymentProvider, repository);
        var promise = new DonationPromise { Provider = "mollie", Amount = 10, Currency = "EUR", Site = "example.com" };
        
        var result = await Endpoints.CreateDonation(serviceProvider.Object, promise);
        
        Assert.IsInstanceOfType<StatusCodeHttpResult>(result);
    }    
   
    [TestMethod]
    public async Task CreateDonation_Success()
    {
        var paymentProvider = new Mock<IPaymentProvider>();
        paymentProvider
            .Setup(s => s.CreateResponseAsync(It.IsAny<string>(), It.IsAny<DonationPromise>()))
            .ReturnsAsync(new DonationResponse { PaymentId = "payment-id", Method = Method.Get, Url = "example.com", Data = paymentProvider });
        
        var repository = new Mock<IDonationRepository>();
        repository.Setup(r => r.Insert(It.IsAny<Donation>())).ReturnsAsync(true);
        
        var serviceProvider = GetServiceProvider(paymentProvider, repository);
        var promise = new DonationPromise { Provider = "mollie", Amount = 10, Currency = "EUR", Site = "example.com" };
        
        var result = await Endpoints.CreateDonation(serviceProvider.Object, promise);
        
        Assert.IsInstanceOfType<JsonHttpResult<Donation>>(result);
    }

    [TestMethod]
    public async Task GetDonationStatus_NoRepository()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var donationId = Guid.NewGuid();
        
        var result = await Endpoints.GetDonationStatus(serviceProvider.Object, donationId);
        
        Assert.IsInstanceOfType<StatusCodeHttpResult>(result);
    }
    
    [TestMethod]
    public async Task GetDonationStatus_DonationNotFound()
    {
        var paymentProvider = new Mock<IPaymentProvider>();
        var repository = new Mock<IDonationRepository>();
        var serviceProvider = GetServiceProvider(paymentProvider, repository);
        var donationId = Guid.NewGuid();
        
        var result = await Endpoints.GetDonationStatus(serviceProvider.Object, donationId);
        
        Assert.IsInstanceOfType<NotFound>(result);
    }
    
    [TestMethod]
    [DataRow(PaymentStatus.Paid)]
    [DataRow(PaymentStatus.Failed)]
    [DataRow(PaymentStatus.Expired)]
    public async Task GetDonationStatus_NoProvider(PaymentStatus status)
    {
        var donation = GetDonation(status);
        var repository = new Mock<IDonationRepository>();
        repository.Setup(r => r.GetById(Guid.Parse(donation.Id))).ReturnsAsync(donation);
        var serviceProvider = GetServiceProvider(repository);
        
        var result = await Endpoints.GetDonationStatus(serviceProvider.Object, Guid.Parse(donation.Id));
        
        Assert.IsInstanceOfType<JsonHttpResult<StatusResult>>(result);
        Assert.AreEqual(status, (result as JsonHttpResult<StatusResult>)?.Value?.Status);
    }
    
    [TestMethod]
    public async Task GetDonationStatus_StatusNotFound()
    {
        var donation = GetDonation(PaymentStatus.Open);
        var repository = new Mock<IDonationRepository>();
        repository.Setup(r => r.GetById(Guid.Parse(donation.Id))).ReturnsAsync(donation);
        var paymentProvider = new Mock<IPaymentProvider>();
        var serviceProvider = GetServiceProvider(paymentProvider, repository);
        
        var result = await Endpoints.GetDonationStatus(serviceProvider.Object, Guid.Parse(donation.Id));
        
        Assert.IsInstanceOfType<NotFound>(result);
    }
    
    [TestMethod]
    public async Task GetDonationStatus_StatusNotUpdated()
    {
        var donation = GetDonation(PaymentStatus.Open);
        var repository = new Mock<IDonationRepository>();
        repository.Setup(r => r.GetById(Guid.Parse(donation.Id))).ReturnsAsync(donation);
        var paymentProvider = new Mock<IPaymentProvider>();
        paymentProvider.Setup(p => p.GetStatus(donation.PaymentId)).ReturnsAsync(PaymentStatus.Expired);
        var serviceProvider = GetServiceProvider(paymentProvider, repository);
        
        var result = await Endpoints.GetDonationStatus(serviceProvider.Object, Guid.Parse(donation.Id));
        
        Assert.IsInstanceOfType<BadRequest>(result);
    }

    [TestMethod]
    public async Task GetDonationStatus_Success()
    {
        var donation = GetDonation(PaymentStatus.Open);
        var repository = new Mock<IDonationRepository>();
        repository.Setup(r => r.GetById(Guid.Parse(donation.Id))).ReturnsAsync(donation);
        repository.Setup(r => r.UpdateStatus(donation, PaymentStatus.Expired)).ReturnsAsync(true);
        var paymentProvider = new Mock<IPaymentProvider>();
        paymentProvider.Setup(p => p.GetStatus(donation.PaymentId)).ReturnsAsync(PaymentStatus.Expired);
        var serviceProvider = GetServiceProvider(paymentProvider, repository);
        
        var result = await Endpoints.GetDonationStatus(serviceProvider.Object, Guid.Parse(donation.Id));

        Assert.IsInstanceOfType<JsonHttpResult<StatusResult>>(result);
        Assert.AreEqual(PaymentStatus.Expired, (result as JsonHttpResult<StatusResult>)?.Value?.Status);
    }

    private static Donation GetDonation(PaymentStatus status)
    {
        var donationId = Guid.NewGuid();
        var paymentId = Guid.NewGuid().ToString();
        return new Donation { Id = donationId.ToString(), Amount = 10, Currency = "EUR", Provider = "mollie", PaymentId = paymentId, Status = status };
    }
    
    private static Mock<IServiceProvider> GetServiceProvider(Mock<IPaymentProvider> provider)
    {
        var paymentProviderFactory = new Mock<IPaymentProviderFactory>();
        paymentProviderFactory.Setup(p => p.Create(It.IsAny<string>())).Returns(provider.Object);
        
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(IPaymentProviderFactory))).Returns(paymentProviderFactory.Object);
        
        return serviceProvider;
    }

    private static Mock<IServiceProvider> GetServiceProvider(Mock<IPaymentProvider> provider, Mock<IDonationRepository> repository)
    {
        var serviceProvider = GetServiceProvider(provider);
        serviceProvider.Setup(x => x.GetService(typeof(IDonationRepository))).Returns(repository.Object);
        return serviceProvider;
    }
    
    private static Mock<IServiceProvider> GetServiceProvider(Mock<IDonationRepository> repository)
    {
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(IDonationRepository))).Returns(repository.Object);
        return serviceProvider;
    }
}