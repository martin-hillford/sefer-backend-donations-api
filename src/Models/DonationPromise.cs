namespace Sefer.Backend.Donations.Api.Models;

public class DonationPromise
{
    /// <summary>
    /// The amount to donate. Should be consistent with the currency.
    /// Should not be negative 
    /// </summary>
    public double Amount { get; set; }

    /// <summary>
    /// A description to be sent to be payment provider 
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The currency in which the donation is made
    /// </summary>
    [MaxLength(5)]
    public string? Currency { get; set; } = "EUR";

    /// <summary>
    /// The site from which the donation has been sent
    /// </summary>
    [MaxLength(255)]
    public string Site { get; set; } = string.Empty;
    
    /// <summary>
    /// The provider to use. e.q. PayPal or mollie
    /// </summary>
    /// <remarks>
    /// Currently only mollie is supported
    /// </remarks>
    [MaxLength(255)]
    public string Provider { get; set; } = "Mollie";
}