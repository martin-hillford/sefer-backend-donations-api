namespace Sefer.Backend.Donations.Api.Test;

[TestClass]
public class SetupTest
{
    [TestMethod]
    public void Setup()
    {
        var app = Api.Setup.CreateApp([]);
        
        // Test if the required services are added
        Assert.IsNotNull(app.Services.GetService(typeof(IDbConnectionProvider)));
        Assert.IsNotNull(app.Services.GetService(typeof(IMollieWrapper)));
        Assert.IsNotNull(app.Services.GetService(typeof(IPaymentProviderFactory)));
        Assert.IsNotNull(app.Services.GetService(typeof(IDonationRepository)));
    }
}