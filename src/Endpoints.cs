namespace Sefer.Backend.Donations.Api;

public static class Endpoints
{
    public static async Task<IResult> CreateDonation(IServiceProvider serviceProvider, [FromBody] DonationPromise promise)
    {
        // Create an internal unique id to track the payment 
        var donationId = Guid.NewGuid().ToString();
        
        // Create the payment provider that will handle the payment
        var provider = GetPaymentProvider(serviceProvider, promise.Provider);
        if (provider == null) return Results.BadRequest();
        
        // check if there is a repository that will save the donations
        var repository = serviceProvider.GetService<IDonationRepository>();
        if (repository == null) return Results.StatusCode(500);
        
        // Use payment provider to do its thing
        var response = await provider.CreateResponseAsync(donationId, promise);
        if(response == null) return Results.BadRequest();

        // Insert the donation in the database - used for tracking donations
        var donation = response.Create(donationId, promise);
        
        // And return the response
        var inserted = await repository.Insert(donation);
        return !inserted ? Results.StatusCode(500) : Results.Json(donation);
    }
    
    public static async Task<IResult> GetDonationStatus(IServiceProvider serviceProvider, Guid donationId)
    {
        // Get the donation that was made. This will
        var repository = serviceProvider.GetService<IDonationRepository>();
        if (repository == null) return Results.StatusCode(500);
        
        var donation = await repository.GetById(donationId);
        if (donation == null) return Results.NotFound();
        
        // Check if this is an end status
        var status = Results.Json(new StatusResult { Status = donation.Status });
        if (donation.Status is PaymentStatus.Paid or PaymentStatus.Failed or PaymentStatus.Expired) return status;
        
        // Create the payment provider that will handle the payment
        var provider = GetPaymentProvider(serviceProvider, donation.Provider);
        if (provider == null) return Results.StatusCode(500);
        
        // If this is not an end-state 
        var currentStatus = await provider.GetStatus(donation.PaymentId);
        if (currentStatus == null) return Results.NotFound();
        
        var saved = await repository.UpdateStatus(donation, currentStatus);
        return !saved
            ? Results.BadRequest() 
            : Results.Json(new StatusResult { Status = (PaymentStatus)currentStatus });
    }

    private static IPaymentProvider? GetPaymentProvider(IServiceProvider serviceProvider, string providerName)
    {
        var factory = serviceProvider.GetService<IPaymentProviderFactory>();
        return factory?.Create(providerName);
    }
}