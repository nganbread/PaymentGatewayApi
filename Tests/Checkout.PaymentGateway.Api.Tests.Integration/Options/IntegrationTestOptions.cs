namespace Checkout.PaymentGateway.Api.Tests.Integration.Options
{
    public class IntegrationTestOptions
    {
        public bool InMemoryPaymentGatewayApi { get; set; }
        public bool InMemoryFakeAcquiringBankApi { get; set; }
        public string PaymentGatewayApiUrl { get; set; }
    }
}
