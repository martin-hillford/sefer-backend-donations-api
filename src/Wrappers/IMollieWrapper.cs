namespace Sefer.Backend.Donations.Api.Wrappers;

// Please note, the purposes of the wrappers is not to completely encapsulate the external libraries (that is
// accomplished by the providers) but wrapper them such that the providers became testable and the implementation
// of the wrappers is minimal
public interface IMollieWrapper
{
    Task<PaymentResponse> GetPaymentAsync( IPaymentClient client, string paymentId, bool testMode = false);

    Task<PaymentResponse> CreatePaymentAsync(IPaymentClient client, PaymentRequest paymentRequest);
}