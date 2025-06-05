namespace Sefer.Backend.Donations.Api.Models;

/// <summary>
/// This class contains instructions on how to continue. Although payment providers do things all different
/// it can be abstracted.
///
/// Because either:
/// 1) A redirect must be done to the website of the provider
/// 2) A get method must be sent to the payment provider
/// 3) A post method must be sent to the payment provider
/// </summary>
public class DonationResponse
{
    /// <summary>
    /// The method the client must follow (redirect, get or post)
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Method Method { get; init; }
    
    /// <summary>
    /// The url the client must use to perform the action
    /// </summary>
    public string Url { get; init; }

    /// <summary>
    /// Optional data to send to client
    /// </summary>
    public object? Data { get; init; } = null;
    
    /// <summary>
    /// The id of payment as provided by the payment provider
    /// </summary>
    [JsonIgnore]
    public required string PaymentId { get; init; }
    
    /// <summary>
    /// Creates a donation object 
    /// </summary>
    /// <param name="donationId">The internal id for this donation</param>
    /// <param name="promise">the promise done by the user></param>
    internal Donation Create(string donationId, DonationPromise promise) =>
        new Donation { Amount = promise.Amount, Provider = promise.Provider, PaymentId = PaymentId, Id = donationId };
}

public enum Method { Redirect, Get, Post }