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

        // build the app
        return builder.Build();
    }
}