using Checkout.PaymentGateway.Api.Mvc;
using Checkout.PaymentGateway.Api.Services;
using Checkout.PaymentGateway.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Checkout.PaymentGateway.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPaymentGatewayApi(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<DummyAuthenticationMiddleware>()
                .AddSingleton<IUserContext, HttpContextUserContext>();

            return serviceCollection;
        }
    }
}
