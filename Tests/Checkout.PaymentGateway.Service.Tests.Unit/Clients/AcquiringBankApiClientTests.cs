using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Service.Clients;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Checkout.PaymentGateway.Service.Tests.Unit.Clients
{
    public class AcquiringBankApiClientTests
    {
        private readonly AcquiringBankApiClient _client;
        private readonly MockHttpMessageHandler _mockHttpMessageHandler;

        public AcquiringBankApiClientTests()
        {
            var logger = new Mock<ILogger<AcquiringBankApiClient>>();

            _mockHttpMessageHandler = new MockHttpMessageHandler();
            _client = new AcquiringBankApiClient(new HttpClient(_mockHttpMessageHandler){BaseAddress = new Uri("http://google.com") }, logger.Object);
        }   

        [Fact]
        public async Task WhenHttpResponseIsSuccessful_ThenAResponseWithAValueIsReturned()
        {
            _mockHttpMessageHandler.Setup(() => new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                Content = new StringContent("1", Encoding.UTF8, "application/json")
            });

            var response = await _client.Post<int>("/", null);

            response.IsSuccessful.Should().BeTrue();
            response.Value.Should().Be(1);
        }

        [Fact]
        public async Task WhenHttpResponseIsNotSuccessful_ThenAnUnsuccessfulResponseIsReturned()
        {
            _mockHttpMessageHandler.Setup(() => new HttpResponseMessage(HttpStatusCode.BadRequest));

            var response = await _client.Post<int>("/", null);

            response.IsSuccessful.Should().BeFalse();
        }

        [Fact]
        public async Task GivenServerIsntRunning_WhenANestedSockedExceptioIsThrown_ThenAnUnsuccessfulResponseIsReturned()
        {
            _mockHttpMessageHandler.Setup(() => throw new HttpRequestException("", new SocketException()));

            var response = await _client.Post<int>("/", null);

            response.IsSuccessful.Should().BeFalse();
        }
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private Func<HttpResponseMessage> _action;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_action == null) throw new Exception($"Ensure that {nameof(MockHttpMessageHandler)}.{nameof(Setup)}() has been called before use");

            return _action();
        }

        public void Setup(Func<HttpResponseMessage> action) => _action = action;
    }
}
