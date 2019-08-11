using System;
using System.Linq;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Service.Clients;
using Checkout.PaymentGateway.Service.Domain;
using Checkout.PaymentGateway.Service.Eventing;
using Checkout.PaymentGateway.Service.Eventing.Events.Payment;
using Checkout.PaymentGateway.Service.RequestResponse;

namespace Checkout.PaymentGateway.Service.Services
{
    internal class PaymentService : IPaymentService
    {
        private readonly IEventStore<Payment> _eventStore;
        private readonly IEventDispatcher<Payment> _eventDispatcher;
        private readonly IUserContext _userContext;
        private readonly IAcquiringBankApiClient _acquiringBankApiClient;

        public PaymentService(IEventStore<Payment> eventStore, IEventDispatcher<Payment> eventDispatcher, IUserContext userContext, IAcquiringBankApiClient acquiringBankApiClient)
        {
            _eventStore = eventStore;
            _eventDispatcher = eventDispatcher;
            _userContext = userContext;
            _acquiringBankApiClient = acquiringBankApiClient;
        }

        /// <summary>
        /// Even though all of the events are fired in one method, as the payment flow becomes
        /// more complicated (adding sanctions screening, non-trivial validation, eventual consistency etc) it
        /// would be 'simple' to refactor the service, audit the payment and add further events.
        /// </summary>
        public async Task<Response> Create(CreateRequest request)
        {
            var payment = await Get(request.Id);
            if (payment != null) return new Response($"A payment with Id '{request.Id}' already exists");

            await _eventDispatcher.Dispatch(new CreatePaymentEvent(request.Id, DateTime.UtcNow, request.Amount, request.Currency, request.CardNumber));
            await _eventDispatcher.Dispatch(new SentToAcquiringBankEvent(request.Id, DateTime.UtcNow));

            var response = await _acquiringBankApiClient.Post<Guid>("/payment", new
            {
                request.Amount,
                request.CardNumber,
                request.Currency,
                request.ExpiryMonth,
                request.ExpiryYear,
                request.SecurityCode
            });

            if (response.IsSuccessful)
            {
                await _eventDispatcher.Dispatch(new PaymentSuccessEvent(request.Id, DateTime.UtcNow, response.Value));
            }
            else
            {
                await _eventDispatcher.Dispatch(new PaymentFailureEvent(request.Id, DateTime.UtcNow, response.ToString()));
            }

            return response;
        }

        public async Task<Payment> Get(Guid id)
        {
            var events = await _eventStore.Get(_userContext.UserName, id);

            if (!events.Any()) return null;

            var payment = new Payment();

            foreach (var @event in events.OrderBy(x => x.TimeStamp))
            {
                @event.Process(payment);
            }

            return payment;
        }
    }
}