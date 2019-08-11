namespace Checkout.PaymentGateway.Service.Domain
{
    public enum PaymentStatus
    {
        Unknown,
        Created,
        WithAcquiringBank,
        Success,
        Failure
    }
}