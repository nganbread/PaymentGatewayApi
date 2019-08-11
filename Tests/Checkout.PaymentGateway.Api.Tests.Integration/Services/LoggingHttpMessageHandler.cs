using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Checkout.PaymentGateway.Api.Tests.Integration.Services
{
    internal class LoggingHttpMessageHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingHttpMessageHandler> _logger;

        public LoggingHttpMessageHandler(ILogger<LoggingHttpMessageHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"HTTP Request: Method={request.Method},RequestUri={request.RequestUri},Headers{request.Headers}");

            var response = await base.SendAsync(request, cancellationToken);

            _logger.LogInformation($"HTTP Response: StatusCode={response.StatusCode},Headers{response.Headers}");

            return response;
        }
    }
}