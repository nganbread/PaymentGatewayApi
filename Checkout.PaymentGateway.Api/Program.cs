using Microsoft.AspNetCore.Hosting;

namespace Checkout.PaymentGateway.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PaymentGatewayHostBuilder
                .Create()
                .Build()
                .Run();
        }
    }
}
