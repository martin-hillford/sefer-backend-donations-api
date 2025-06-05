namespace Sefer.Backend.Donations.Api.Providers;

public class IdealMollieProvider(IServiceProvider serviceProvider) : PaymentProvider(serviceProvider)
{
    private readonly IOptions<PaymentOptions>? _options = serviceProvider.GetService<IOptions<PaymentOptions>>();
    
    private readonly IMollieWrapper? _wrapper = serviceProvider.GetService<IMollieWrapper>();

    public override async Task<DonationResponse?> CreateResponseAsync(string donationId, DonationPromise promise)
    {
        // Check all the incoming data. It may seem redundant to check it here but (possible) different providers
        // have different requirements on the input 
        var site = await GetSite(promise.Site);
        var currency = GetCurrency(promise);
        var amount = GetAmount(promise);
        if (currency == null || amount == null || site == null) return null;

        // Using the mollie payment client to prepare the payment
        var paymentClient = GetPaymentClient();
        if (paymentClient == null || _wrapper == null) return null;
        var paymentRequest = new PaymentRequest
        {
            Amount = new Amount(Currency.EUR, amount),
            Description = promise.Description,
            RedirectUrl = site.SiteUrl + "/donate/feedback/" + donationId
        };
        var paymentResponse = await _wrapper.CreatePaymentAsync(paymentClient, paymentRequest);
        var checkoutLink = paymentResponse.Links.Checkout?.Href;

        // Check if mollie has returned the expected 
        if (checkoutLink == null) return null;
        return new DonationResponse
        {
            Method = Method.Redirect,
            Url = checkoutLink,
            PaymentId = paymentResponse.Id
        };
    }

    public override async Task<PaymentStatus?> GetStatus(string paymentId)
    {
        var paymentClient = GetPaymentClient();
        if (paymentClient == null || _wrapper == null) return null;
        var response = await _wrapper.GetPaymentAsync(paymentClient, paymentId);
        return response.Status.ToLower() switch
        {
            "canceled" => PaymentStatus.Canceled,
            "pending" => PaymentStatus.Pending,
            "authorized" => PaymentStatus.Authorized,
            "expired" => PaymentStatus.Expired,
            "failed" => PaymentStatus.Failed,
            "paid" => PaymentStatus.Paid,
            _ => PaymentStatus.Open
        };
    }

    private PaymentClient? GetPaymentClient()
        => _options?.Value.Mollie?.ApiKey != null
            ? new PaymentClient(_options.Value.Mollie.ApiKey)
            : null;

    private static string? GetCurrency(DonationPromise promise)
    {
        return promise.Currency?.ToLower() switch
        {
            "eur" => Currency.EUR,
            "usd" => Currency.USD,
            "gbp" => Currency.GBP,
            "chf" => Currency.CHF,
            _ => null
        };
    }

    private static string? GetAmount(DonationPromise promise)
        => promise.Amount < 1 ? null : promise.Amount.ToString("0.00");
}