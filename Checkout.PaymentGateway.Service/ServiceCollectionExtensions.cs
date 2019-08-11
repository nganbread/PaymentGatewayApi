using Checkout.PaymentGateway.Service.Clients;
using Checkout.PaymentGateway.Service.Eventing;
using Checkout.PaymentGateway.Service.Options;
using Checkout.PaymentGateway.Service.Serialization;
using Checkout.PaymentGateway.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Checkout.PaymentGateway.Service
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPaymentGatewayServices(this IServiceCollection services)
        {
            services
                .AddSingleton<IJsonSerializer, JsonSerializer>()
                .AddSingleton(typeof(IEventDispatcher<>), typeof(EventDispatcher<>))
                .AddSingleton(typeof(IEventStore<>), typeof(EventStore<>))
                .AddScoped<IPaymentService, PaymentService>()
                .AddHttpClient<IAcquiringBankApiClient, AcquiringBankApiClient>((serviceProvider, httpClient) =>
                {
                    var acquiringBankApi = serviceProvider.GetRequiredService<IOptions<AcquiringBankApiOptions>>();

                    httpClient.BaseAddress = acquiringBankApi.Value.Uri;
                });

            return services;
        }
    }
}
