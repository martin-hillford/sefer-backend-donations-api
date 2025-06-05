namespace Sefer.Backend.Donations.Api.Test.Providers;

[TestClass]
public class PaymentProviderFactoryTest
{
    [TestMethod]
    public void Create_Mollie()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var factory = new PaymentProviderFactory(serviceProvider.Object);
        
        var provider = factory.Create("Mollie");
        
        Assert.IsNotNull(provider);
        Assert.IsInstanceOfType(provider, typeof(IdealMollieProvider));
    }

    [TestMethod]
    public void Create_Null()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var factory = new PaymentProviderFactory(serviceProvider.Object);
        
        var provider = factory.Create("null");
        
        Assert.IsNull(provider);
    }
}