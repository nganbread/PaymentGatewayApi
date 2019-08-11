using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Checkout.PaymentGateway.Api
{
    internal static class PaymentGatewayHostBuilder
    {
        public static IWebHostBuilder Create()
        {
            return new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var hostingEnvironment = context.HostingEnvironment;
                    builder
                        .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();

                    if (hostingEnvironment.IsProduction())
                    {
                        //Add secrets from a secure store such as KeyVault
                    }
                })
                .ConfigureLogging((context, builder) =>
                {
                    if (context.HostingEnvironment.IsProduction())
                    {
                        //Add production logging
                        builder
                            .AddConsole();
                    }
                    else
                    {
                        builder
                            .AddConsole()
                            .AddDebug()
                            .AddEventSourceLogger();
                    }
                })
                .UseStartup<Startup>();
        }
    }
}