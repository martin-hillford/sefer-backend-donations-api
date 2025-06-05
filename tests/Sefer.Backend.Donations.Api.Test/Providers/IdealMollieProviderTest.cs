using Mode = Mollie.Api.Models.Mode;

namespace Sefer.Backend.Donations.Api.Test.Providers;

[TestClass]
public class IdealMollieProviderTest
{
    [TestMethod]
    public async Task CreateResponseAsync_NoNetworkProvider()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var provider = new IdealMollieProvider(serviceProvider.Object);
        var promise = new DonationPromise { Amount = 10.0, Currency = "EUR", Description = "description", Provider = "mollie", Site = "site" };
        
        var response = await provider.CreateResponseAsync(Guid.NewGuid().ToString(), promise);
        
        Assert.IsNull(response);
    }

    [TestMethod]
    public async Task CreateResponseAsync_NoSite()
    {
        var serviceProvider = GetServiceProvider();
        var provider = new IdealMollieProvider(serviceProvider.Object);
        var promise = new DonationPromise { Amount = 10.0, Currency = "EUR", Provider = "mollie", Site = "no-site" };

        var response = await provider.CreateResponseAsync(Guid.NewGuid().ToString(), promise);
        
        Assert.IsNull(response);
    }
    
    [TestMethod]
    public async Task CreateResponseAsync_NoCurrency()
    {
        var serviceProvider = GetServiceProvider();
        var provider = new IdealMollieProvider(serviceProvider.Object);
        var promise = new DonationPromise { Amount = 10.0, Currency = null, Provider = "mollie", Site = "site" };

        var response = await provider.CreateResponseAsync(Guid.NewGuid().ToString(), promise);
        
        Assert.IsNull(response);
    }
    
    [TestMethod]
    public async Task CreateResponseAsync_NoAmount()
    {
        var serviceProvider = GetServiceProvider();
        var provider = new IdealMollieProvider(serviceProvider.Object);
        var promise = new DonationPromise { Amount = -1, Currency = "EUR", Provider = "mollie", Site = "site" };

        var response = await provider.CreateResponseAsync(Guid.NewGuid().ToString(), promise);
        
        Assert.IsNull(response);
    }
    
    [TestMethod]
    public async Task CreateResponseAsync_NoPaymentClient()
    {
        var serviceProvider = GetServiceProvider().AddNetworkProvider();
        var provider = new IdealMollieProvider(serviceProvider.Object);
        var promise = new DonationPromise { Amount = 10.0, Currency = "EUR", Provider = "mollie", Site = "site" };

        var response = await provider.CreateResponseAsync(Guid.NewGuid().ToString(), promise);
        
        Assert.IsNull(response);
    }
    
    [TestMethod]
    public async Task CreateResponseAsync_NoWrapper()
    {
        var serviceProvider = GetServiceProvider().AddNetworkProvider().AddPaymentClientInfo();
        var provider = new IdealMollieProvider(serviceProvider.Object);
        var promise = new DonationPromise { Amount = 10.0, Currency = "GBP", Provider = "mollie", Site = "site" };

        var response = await provider.CreateResponseAsync(Guid.NewGuid().ToString(), promise);
        
        Assert.IsNull(response);
    }

    [TestMethod]
    public async Task CreateResponseAsync_NoCheckoutLink()
    {
        var response = GetPaymentResponse();
        var serviceProvider = GetServiceProvider()
            .AddNetworkProvider().AddPaymentClientInfo().AddMollieWrapper(response);
        var provider = new IdealMollieProvider(serviceProvider.Object);
        var promise = new DonationPromise { Amount = 10.0, Currency = "CHF", Provider = "mollie", Site = "site" };

        var created = await provider.CreateResponseAsync(Guid.NewGuid().ToString(), promise);
        
        Assert.IsNull(created);
    }
    
    [TestMethod]
    public async Task CreateResponseAsync_Success()
    {
        var response = GetPaymentResponse("http://checkoutlink");
        var serviceProvider = GetServiceProvider()
            .AddNetworkProvider().AddPaymentClientInfo().AddMollieWrapper(response);
        var provider = new IdealMollieProvider(serviceProvider.Object);
        var promise = new DonationPromise { Amount = 10.0, Currency = "USD", Provider = "mollie", Site = "site" };

        var created = await provider.CreateResponseAsync(Guid.NewGuid().ToString(), promise);
        
        Assert.IsNotNull(created);
        Assert.AreEqual(Method.Redirect, created.Method);
        Assert.AreEqual("http://checkoutlink", created.Url);
        Assert.IsNull(created.Data);
    }
    
    [TestMethod]
    public async Task GetStatus_NoPaymentClient()
    {
        var serviceProvider = GetServiceProvider().AddNetworkProvider();
        var provider = new IdealMollieProvider(serviceProvider.Object);
        
        var paymentStatus = await provider.GetStatus("paymentId");
        
        Assert.IsNull(paymentStatus);
    }
    
    [TestMethod]
    public async Task GetStatus_NoWrapper()
    {
        var serviceProvider = GetServiceProvider().AddNetworkProvider().AddPaymentClientInfo();
        var provider = new IdealMollieProvider(serviceProvider.Object);
        
        var paymentStatus = await provider.GetStatus("paymentId");
        
        Assert.IsNull(paymentStatus);
    }

    [TestMethod]
    [DataRow("canceled", PaymentStatus.Canceled)]
    [DataRow("pending", PaymentStatus.Pending)]
    [DataRow("authorized", PaymentStatus.Authorized)]
    [DataRow("expired", PaymentStatus.Expired)]
    [DataRow("failed", PaymentStatus.Failed)]
    [DataRow("paid", PaymentStatus.Paid)]
    [DataRow("unset", PaymentStatus.Open)]
    [DataRow("unknown", PaymentStatus.Open)]
    public async Task GetStatus_Success(string status, PaymentStatus expected)
    {
        var response = GetPaymentResponse(null, status);
        var serviceProvider = GetServiceProvider()
            .AddNetworkProvider().AddPaymentClientInfo().AddMollieWrapper(response);
        
        var provider = new IdealMollieProvider(serviceProvider.Object);
        var paymentStatus = await provider.GetStatus("paymentId");
        Assert.AreEqual(expected, paymentStatus);
    }
    
    private static Mock<IServiceProvider> GetServiceProvider() => new Mock<IServiceProvider>();

    private static PaymentResponse GetPaymentResponse(string? checkoutLink = null, string status = "status" )
    {
        var response = new PaymentResponse
        {
            WebhookUrl = "hook", Id = "id", Amount = new Amount("EUR", 10), Mode = Mode.Test,
            CreatedAt = DateTime.UtcNow, Status = status, Resource = "resource", ProfileId = "profile_id",
            SequenceType = "sequence_type",
            Links = new PaymentResponseLinks
            {
                Self = new UrlObjectLink<PaymentResponse> { Href = "href", Type = "type" },
                Dashboard = new UrlLink { Href = "href", Type = "type" },
                Documentation = new UrlLink { Href = "href", Type = "type" },
            },
        };

        if (checkoutLink != null) response.Links.Checkout = new UrlLink { Href = checkoutLink, Type = "type" };
        return response;
    }
}

internal static class IdealMollieProviderExtensions
{
    internal static Mock<IServiceProvider> AddNetworkProvider(this Mock<IServiceProvider> serviceProvider)
    {
        var networkProvider = new Mock<INetworkProvider>();
        var site = new Mock<ISite>(); site.Setup(s => s.Name).Returns("site"); site.Setup(s => s.SiteUrl).Returns("https://site.com");
        networkProvider.Setup(x => x.GetSitesAsync()).ReturnsAsync([site.Object]);
        serviceProvider.Setup(x => x.GetService(typeof(INetworkProvider))).Returns(networkProvider.Object);
        return serviceProvider;
    }

    internal static Mock<IServiceProvider> AddPaymentClientInfo(this Mock<IServiceProvider> serviceProvider)
    {
        var mollie = new Mollie { ApiKey = "api-key" };
        var payment = new PaymentOptions { Mollie = mollie  };
        var options = Options.Create(payment);
        serviceProvider.Setup(x => x.GetService(typeof(IOptions<PaymentOptions>))).Returns(options);
        return serviceProvider;
    }
    
    internal static Mock<IServiceProvider> AddMollieWrapper(this Mock<IServiceProvider> serviceProvider, PaymentResponse paymentResponse)
    {
        var mollieWrapper = new Mock<IMollieWrapper>();
        mollieWrapper
            .Setup(w => w.CreatePaymentAsync(It.IsAny<PaymentClient>(), It.IsAny<PaymentRequest>()))
            .ReturnsAsync(paymentResponse);
        
        mollieWrapper
            .Setup(w => w.GetPaymentAsync(It.IsAny<PaymentClient>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(paymentResponse);
        serviceProvider.Setup(x => x.GetService(typeof(IMollieWrapper))).Returns(mollieWrapper.Object);
        return serviceProvider;
    }
    
}