using System.Threading.Tasks;
using Checkout.PaymentGateway.Service.RequestResponse;

namespace Checkout.PaymentGateway.Service.Clients
{
    internal interface IAcquiringBankApiClient
    {
        Task<Response<T>> Post<T>(string uri, object body);
    }
}