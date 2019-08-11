using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Api.Tests.Integration.V1.Builders;
using Checkout.PaymentGateway.Api.V1.Payment;
using Checkout.PaymentGateway.Service.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.PaymentGateway.Api.Tests.Integration.V1
{
    public class PaymentTests
    {
        private readonly HttpClient _apiClient;
        private readonly ITestOutputHelper _testOutputHelper;

        public PaymentTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var testHost = IntegrationTestHostBuilder.Create(testOutputHelper).Build();

            var clientFactory = testHost.Services.GetRequiredService<IHttpClientFactory>();
            _apiClient = clientFactory.CreateClient("PaymentGatewayApi");
            _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("IntegrationTestUser1");
        }      

        [Fact]
        public async Task GivenPaymentCreation_WhenSendingAValidRequest_ThenTheResponseHasA201CreatedStatusCode()
        {
            var paymentId = Guid.NewGuid();
            var model = new PaymentPostModelBuilder().Build();

            var postResponse = await _apiClient.PostAsJsonAsync($"/v1/payment/{paymentId}", model);
            postResponse?.StatusCode.Should().Be(StatusCodes.Status201Created);

            var getResponse = await _apiClient.GetAsync($"/v1/payment/{paymentId}");
            getResponse?.StatusCode.Should().Be(StatusCodes.Status200OK);

            var payment = await getResponse.Content.ReadAsAsync<PaymentGetModel>();

            payment.Id.Should().Be(paymentId);
            payment.Status.Should().Be(PaymentStatus.Success);
        }
        
        [Fact]
        public async Task GivenPaymentCreation_WhenSendingAnInvalidRequest_ThenTheResponseHasA400BadRequestErrorStatusCodeAndTheRetrievedPaymentIsNotSuccessful()
        {
            var paymentId = Guid.NewGuid();
            var model = new PaymentPostModelBuilder()
                .WithFailingCardNumber()
                .Build();

            var postResponse = await _apiClient.PostAsJsonAsync($"/v1/payment/{paymentId}", model);
            postResponse?.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var getResponse = await _apiClient.GetAsync($"/v1/payment/{paymentId}");
            getResponse?.StatusCode.Should().Be(StatusCodes.Status200OK);

            var payment = await getResponse.Content.ReadAsAsync<PaymentGetModel>();

            payment.Id.Should().Be(paymentId);
            payment.Status.Should().Be(PaymentStatus.Failure);
        }
        
        [Fact]
        public async Task GivenPaymentCreation_WhenSendingARequestWithTheSameIdTwice_ThenTheSecondResponseHasA400BadRequestStatusCode()
        {
            var paymentId = Guid.NewGuid();
            var request = new PaymentPostModelBuilder().Build();

            var response1 = await _apiClient.PostAsJsonAsync($"/v1/payment/{paymentId}", request);
            response1?.StatusCode.Should().Be(StatusCodes.Status201Created);

            var response2 = await _apiClient.PostAsJsonAsync($"/v1/payment/{paymentId}", request);
            response2?.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
        
        [Fact]
        public async Task GivenPaymentCreation_WhenSendingARequestWithTheSameIdTwiceWithDifferentUsers_ThenBothResponsesHave201CreatedStatusCodes()
        {
            var paymentId = Guid.NewGuid();
            var request = new PaymentPostModelBuilder().Build();

            var response1 = await _apiClient.PostAsJsonAsync($"/v1/payment/{paymentId}", request);
            response1?.StatusCode.Should().Be(StatusCodes.Status201Created);

            _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("IntegrationTestUser2");

            var response2 = await _apiClient.PostAsJsonAsync($"/v1/payment/{paymentId}", request);
            response2?.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Fact]
        public async Task GivenPaymentCreation_WhenRetrievingThePaymentAsADifferentUser_ThenTheResponseHas404NotFoundStatusCode()
        {
            var paymentId = Guid.NewGuid();
            var model = new PaymentPostModelBuilder().Build();

            var response1 = await _apiClient.PostAsJsonAsync($"/v1/payment/{paymentId}", model);
            response1?.StatusCode.Should().Be(StatusCodes.Status201Created);

            _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("IntegrationTestUser2");

            var response2 = await _apiClient.GetAsync($"/v1/payment/{paymentId}");
            response2?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Theory]
        [MemberData(nameof(InvalidPaymentPostModels))]
        public async Task WhenSendingAnInvalidRequest_ThenTheResponseHas500BadRequestCodeAndAPaymentCanNotBeRetrieved(PaymentPostModel model)
        {
            var paymentId = Guid.NewGuid();

            _testOutputHelper.WriteLine($"Sending payment {paymentId} with body {JsonConvert.SerializeObject(model)}");

            var response1 = await _apiClient.PostAsJsonAsync($"/v1/payment/{paymentId}", model);
            response1?.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var response2 = await _apiClient.GetAsync($"/v1/payment/{paymentId}");
            response2?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task GivenRetrieval_WhenSendingAnUnknownPayment_ThenTheResponseHasA404NotFoundStatusCode()
        {
            var response = await _apiClient.GetAsync($"/v1/payment/{Guid.NewGuid()}");

            response?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        public static IEnumerable<object[]> InvalidPaymentPostModels => InvalidPaymentPostModelValues().Select(x => new[] { x });

        private static IEnumerable<object> InvalidPaymentPostModelValues()
        {
            yield return new PaymentPostModelBuilder().With(x => x.CardNumber = null).Build();
            yield return new PaymentPostModelBuilder().With(x => x.CardNumber = "").Build();
            yield return new PaymentPostModelBuilder().With(x => x.CardNumber = "12345678901234567890").Build();
            yield return new PaymentPostModelBuilder().With(x => x.Amount = -1).Build();
            yield return new PaymentPostModelBuilder().With(x => x.Currency = Currency.Unknown).Build();
            yield return new PaymentPostModelBuilder().With(x => x.Currency = (Currency)(-1)).Build();
            yield return new PaymentPostModelBuilder().With(x => x.ExpiryMonth = 0).Build();
            yield return new PaymentPostModelBuilder().With(x => x.ExpiryMonth = 13).Build();
            yield return new PaymentPostModelBuilder().With(x => x.ExpiryYear = 3000).Build();
            yield return new PaymentPostModelBuilder().With(x => x.ExpiryYear = 2018).Build();
            yield return new PaymentPostModelBuilder().With(x => x.SecurityCode = null).Build();
            yield return new PaymentPostModelBuilder().With(x => x.SecurityCode = "").Build();
            yield return new PaymentPostModelBuilder().With(x => x.SecurityCode = "12345").Build();
        }

    }
}
