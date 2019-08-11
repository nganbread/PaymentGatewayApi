using Microsoft.AspNetCore.Hosting;

namespace Checkout.PaymentGateway.Fake.AcquiringBank.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FakeAcquiringBankHostBuilder
                .Create()
                .Build()
                .Run();
        }
    }
}
