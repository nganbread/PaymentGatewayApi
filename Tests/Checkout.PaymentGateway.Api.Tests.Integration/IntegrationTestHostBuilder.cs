using System;
using System.IO;
using System.Net.Http;
using Checkout.PaymentGateway.Api.Tests.Integration.Options;
using Checkout.PaymentGateway.Api.Tests.Integration.Services;
using Checkout.PaymentGateway.Fake.AcquiringBank.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Checkout.PaymentGateway.Api.Tests.Integration
{
    internal static class IntegrationTestHostBuilder
    {
        /// <summary>
        /// We're not hosting anything, but using a HostBuilder makes it easy to configure our tests and set up services
        /// </summary>
        public static IHostBuilder Create(ITestOutputHelper testOutputHelper)
        {
            return new HostBuilder()
                .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var hostingEnvironment = context.HostingEnvironment;
                    builder
                        .AddJsonFile($"appsettings.integration.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.integration.{hostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables();

                    testOutputHelper.WriteLine($"ConfigureAppConfiguration: HostingEnvironment={hostingEnvironment.EnvironmentName}");
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder
                        .AddConsole()
                        .AddProvider(new TestOutputHelperLoggerProvider(testOutputHelper));

                    testOutputHelper.WriteLine($"ConfigureLogging:");
                })
                .ConfigureServices((context, services) =>
                {
                    var section = context.Configuration.GetSection("IntegrationTests");
                    var options = section.Get<IntegrationTestOptions>();

                    services
                        .AddSingleton(testOutputHelper)
                        .AddSingleton<LoggingHttpMessageHandler>();

                    if (options.InMemoryPaymentGatewayApi)
                    {
                        services.AddInMemoryPaymentGatewayApi(testOutputHelper, options.InMemoryFakeAcquiringBankApi);
                    }
                    else
                    {
                        services.AddPaymentGatewayApi(options.PaymentGatewayApiUrl);
                    }

                    testOutputHelper.WriteLine($"ConfigureServices:InMemoryPaymentGatewayApi={options.InMemoryPaymentGatewayApi},InMemoryFakeAcquiringBankApi={options.InMemoryFakeAcquiringBankApi},PaymentGatewayApiUrl={options.PaymentGatewayApiUrl}");
                });
        }

        private static IServiceCollection AddPaymentGatewayApi(this IServiceCollection services, string paymentGatewayApiUrl)
        {
            services
                .AddHttpClient("PaymentGatewayApi", httpClient =>
                {
                    httpClient.BaseAddress = new Uri(paymentGatewayApiUrl);
                })
                .AddHttpMessageHandler<LoggingHttpMessageHandler>();

            return services;
        }

        /// <summary>
        /// Setup a host to communicate with an in memory payment gateway. Optionally configures the in memory payment gateway to talk to an in memory fake acquiring bank
        /// </summary>
        private static IServiceCollection AddInMemoryPaymentGatewayApi(this IServiceCollection services, ITestOutputHelper testOutputHelper, bool inMemoryFakeAcquiringBankApi)
        {
            var paymentGatewayHostBuilder = PaymentGatewayHostBuilder
                .Create()
                .ConfigureLogging((paymentGatewayContext, loggingBuilder) =>
                {
                    loggingBuilder
                        .AddConsole()
                        .AddProvider(new TestOutputHelperLoggerProvider(testOutputHelper));
                })
                .ConfigureServices((context, paymentGatewayServices) =>
                {
                    if (inMemoryFakeAcquiringBankApi)
                    {
                        paymentGatewayServices.AddInMemoryFakeAcquiringBankApi(testOutputHelper);
                    }
                });

            var paymentGatewayApiTestServer = new TestServer(paymentGatewayHostBuilder);
            var paymentGatewayHttpClientFactory = new TestServerHttpClientFactory(paymentGatewayApiTestServer);

            return services.AddSingleton<IHttpClientFactory>(paymentGatewayHttpClientFactory);
        }

        /// <summary>
        /// Setup a webhost to communicate with an in memory acquiring bank
        /// </summary>
        private static IServiceCollection AddInMemoryFakeAcquiringBankApi(this IServiceCollection services, ITestOutputHelper testOutputHelper)
        {
            var fakeAcquiringBankHostBuilder = FakeAcquiringBankHostBuilder
                .Create()
                .ConfigureLogging((paymentGatewayContext, loggingBuilder) =>
                {
                    loggingBuilder.AddProvider(new TestOutputHelperLoggerProvider(testOutputHelper));
                });

            var fakeAcquiringBankApiTestServer = new TestServer(fakeAcquiringBankHostBuilder);
            var fakeAcquiringBankHttpClientFactory = new TestServerHttpClientFactory(fakeAcquiringBankApiTestServer);

            return services.AddSingleton<IHttpClientFactory>(fakeAcquiringBankHttpClientFactory);
        }
    }
}