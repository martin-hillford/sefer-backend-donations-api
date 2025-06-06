namespace Sefer.Backend.Donations.Api;

public static class Setup
{
    public static WebApplication CreateApp(string[] args)
    {
        // Create the builder and use the shared config
        var builder = WebApplication
            .CreateBuilder(args)
            .WithSharedConfig()
            .AddTokenAuthentication();
        
        // Add the configuration 
        builder.Services.Configure<PaymentOptions>(builder.Configuration.GetSection("Payment"));
        
        // Add the services
        builder.Services.AddSingleton<IDbConnectionProvider, DbConnectionProvider>();
        builder.Services.AddSingleton<IMollieWrapper, MollieWrapper>();
        builder.Services.AddSingleton<IPaymentProviderFactory, PaymentProviderFactory>();
        builder.Services.AddSingleton<IDonationRepository, DonationRepository>();

        // build the app
        return builder.Build();
    }
}