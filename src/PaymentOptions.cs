namespace Sefer.Backend.Donations.Api;

public class PaymentOptions
{
    /// <summary>
    /// Options set for mollie
    /// </summary>
    public Mollie? Mollie { get; set; }
}

public class Mollie
{
    /// <summary>
    /// The api-key to use with mollie
    /// </summary>
    public string? ApiKey { get; set;}
}