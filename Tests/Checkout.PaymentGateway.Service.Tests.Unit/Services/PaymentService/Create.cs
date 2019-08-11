using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Service.Clients;
using Checkout.PaymentGateway.Service.Domain;
using Checkout.PaymentGateway.Service.Eventing;
using Checkout.PaymentGateway.Service.Eventing.Events.Payment;
using Checkout.PaymentGateway.Service.RequestResponse;
using Checkout.PaymentGateway.Service.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Checkout.PaymentGateway.Service.Tests.Unit.Services.PaymentService
{
    public class Create
    {
        private readonly Service.Services.PaymentService _paymentService;
        private readonly IList<IEvent<Payment>> _events = new List<IEvent<Payment>>();
        private readonly Mock<IEventDispatcher<Payment>> _eventDispatcher;
        private readonly Mock<IAcquiringBankApiClient> _acquiringBankApiClient;

        public Create()
        {
            var userContext = new Mock<IUserContext>();
            var eventStore = new Mock<IEventStore<Payment>>();

            _acquiringBankApiClient = new Mock<IAcquiringBankApiClient>();
            _eventDispatcher = new Mock<IEventDispatcher<Payment>>();
            _paymentService = new Service.Services.PaymentService(eventStore.Object, _eventDispatcher.Object, userContext.Object, _acquiringBankApiClient.Object);
            
            eventStore
                .Setup(x => x.Get(It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(() => _events);

            _acquiringBankApiClient
                .Setup(x => x.Post<Guid>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new Response<Guid>(Guid.Empty));
        }

        [Fact]
        public async Task WhenPaymentExists_ThenAnUnsuccessfulResponseIsReturnedAndNoEventsAreRaisedAndNoRequestIsSentToTheAcquiringBank()
        {
            _events.Add(new Mock<IEvent<Payment>>().Object);

            var response = await _paymentService.Create(new CreateRequest(default, default, default, default, default, default, default));

            response.IsSuccessful.Should().BeFalse();
            _eventDispatcher.Verify(x => x.Dispatch(It.IsAny<IEvent<Payment>>()), Times.Never);
            _acquiringBankApiClient.Verify(x => x.Post<Guid>(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task WhenPaymentDoesntExist_ThenACreateEventIsDispatched()
        {
            var response = await _paymentService.Create(new CreateRequest(default, default, default, default, default, default, default));

            _eventDispatcher.Verify(x => x.Dispatch(It.IsAny<CreatePaymentEvent>()), Times.Once);
        }
        
        [Fact]
        public async Task WhenPaymentDoesntExist_ThenASentToAcquiringBankEventIsDispatched()
        {
            var response = await _paymentService.Create(new CreateRequest(default, default, default, default, default, default, default));

            _eventDispatcher.Verify(x => x.Dispatch(It.IsAny<SentToAcquiringBankEvent>()), Times.Once);
        }
        
        [Fact]
        public async Task GivenPaymentDoesntExist_WhenTheAcquiringBankResponseIsSuccessful_ThenAPaymentSuccessEventIsDispatchedAndSuccessIsReturned()
        {
            var response = await _paymentService.Create(new CreateRequest(default, default, default, default, default, default, default));

            _eventDispatcher.Verify(x => x.Dispatch(It.IsAny<PaymentSuccessEvent>()), Times.Once);
            _eventDispatcher.Verify(x => x.Dispatch(It.IsAny<PaymentFailureEvent>()), Times.Never);
            response.IsSuccessful.Should().BeTrue();
        }
        
        [Fact]
        public async Task GivenPaymentDoesntExist_WhenTheAcquiringBankResponseIsNotSuccessful_ThenAPaymentFailureEventIsDispatchedAndUnsuccessfulIsReturned()
        {
            _acquiringBankApiClient
                .Setup(x => x.Post<Guid>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new Response<Guid>(error: "error"));

            var response = await _paymentService.Create(new CreateRequest(default, default, default, default, default, default, default));

            _eventDispatcher.Verify(x => x.Dispatch(It.IsAny<PaymentSuccessEvent>()), Times.Never);
            _eventDispatcher.Verify(x => x.Dispatch(It.IsAny<PaymentFailureEvent>()), Times.Once);
            response.IsSuccessful.Should().BeFalse();
        }
    }
}