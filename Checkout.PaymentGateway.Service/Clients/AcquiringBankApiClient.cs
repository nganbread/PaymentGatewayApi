using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Service.RequestResponse;
using Microsoft.Extensions.Logging;

namespace Checkout.PaymentGateway.Service.Clients
{
    internal class AcquiringBankApiClient : IAcquiringBankApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AcquiringBankApiClient> _logger;

        public AcquiringBankApiClient(HttpClient httpClient, ILogger<AcquiringBankApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// TODO: Consider having specific methods instead of passing in the URI and body
        /// </summary>
        public async Task<Response<T>> Post<T>(string uri, object body)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(uri, body);

                if (response.IsSuccessStatusCode) return await response.Content.ReadAsAsync<T>();

                //TODO: Map acquiring bank responses to more useful errors
                return new Response<T>(error: response.StatusCode.ToString());
            }
            catch (HttpRequestException e) when (e.InnerException is SocketException)
            {
                _logger.LogCritical(e, $"Failed to connect to acquiring bank");

                return new Response<T>(error: "Unable to process payment");
            }
        }
    }
}
