using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Service.Domain;
using Checkout.PaymentGateway.Service.Eventing;
using Checkout.PaymentGateway.Service.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Checkout.PaymentGateway.Service.Tests.Unit.Services.PaymentService
{
    public class Get
    {
        private readonly Service.Services.PaymentService _paymentService;
        private readonly IList<IEvent<Payment>> _events = new List<IEvent<Payment>>();

        public Get()
        {
            var userContext = new Mock<IUserContext>();
            var eventStore = new Mock<IEventStore<Payment>>();

            eventStore
                .Setup(x => x.Get(It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(() => _events);

            _paymentService = new Service.Services.PaymentService(eventStore.Object, null, userContext.Object, null);
        }

        [Fact]
        public async Task WhenThereAreNoEvents_ThenNoPaymentIsReturnedAsync()
        {
            var payment = await _paymentService.Get(Guid.Empty);

            payment.Should().BeNull();
        }

        [Fact]
        public async Task WhenThereAreEvents_ThenEachEventIsExecutedAgainstAPaymentInOrder()
        {
            var event1 = new MockEvent { TimeStamp = new DateTime(2000, 1, 1)};
            var event2 = new MockEvent { TimeStamp = new DateTime(2000, 1, 2)};
            var event3 = new MockEvent { TimeStamp = new DateTime(2000, 1, 3)};
            
            _events.Add(event2);
            _events.Add(event1);
            _events.Add(event3);

            var payment = await _paymentService.Get(Guid.Empty);

            event1.Payment.Should().Be(payment);
            event2.Payment.Should().Be(payment);
            event3.Payment.Should().Be(payment);

            event1.Sequence.Should().BeLessThan(event2.Sequence);
            event2.Sequence.Should().BeLessThan(event3.Sequence);
        }

        private class MockEvent : IEvent<Payment>
        {
            public Guid Id => throw new NotImplementedException();
            public DateTime TimeStamp { get; set; }
            public Payment Payment { get; set; }
            public int Sequence { get; set; }

            private static int _i = 0;
            public void Process(Payment payment)
            {
                Payment = payment;
                Sequence = ++_i;
            }
        }
    }
}
