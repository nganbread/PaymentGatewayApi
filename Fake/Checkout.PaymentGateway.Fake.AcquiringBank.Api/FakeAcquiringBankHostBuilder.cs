using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Checkout.PaymentGateway.Fake.AcquiringBank.Api
{
    public class FakeAcquiringBankHostBuilder
    {
        public static IWebHostBuilder Create()
        {
            return new WebHostBuilder()
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole();
                })
                .UseKestrel()
                .UseStartup<Startup>();
        }
    }
}