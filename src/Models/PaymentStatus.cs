namespace Sefer.Backend.Donations.Api.Models;

/// <summary>
/// Defines the types of payment
/// </summary>
public enum PaymentStatus : short
{
    Open = 0,
    Canceled = 1,
    Pending = 2,
    Authorized = 3,
    Expired = 4,
    Failed = 5,
    Paid = 6
}