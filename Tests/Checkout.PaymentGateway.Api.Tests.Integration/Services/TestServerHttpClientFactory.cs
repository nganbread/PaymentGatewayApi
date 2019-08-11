using System.Net.Http;
using Microsoft.AspNetCore.TestHost;

namespace Checkout.PaymentGateway.Api.Tests.Integration.Services
{
    internal class TestServerHttpClientFactory : IHttpClientFactory
    {
        private readonly TestServer _testServer;

        public TestServerHttpClientFactory(TestServer testServer)
        {
            _testServer = testServer;
        }

        public HttpClient CreateClient(string name)
        {
            return _testServer.CreateClient();
        }
    }
}