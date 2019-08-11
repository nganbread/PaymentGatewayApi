using System.Threading.Tasks;
using Checkout.PaymentGateway.Service.Services;

namespace Checkout.PaymentGateway.Service.Eventing
{
    internal class EventDispatcher<T> : IEventDispatcher<T>
    {
        private readonly IEventStore<T> _eventStore;
        private readonly IUserContext _userContext;

        public EventDispatcher(IUserContext userContext, IEventStore<T> eventStore)
        {
            _userContext = userContext;
            _eventStore = eventStore;
        }

        public Task Dispatch(IEvent<T> @event)
        {
            _eventStore.Add(_userContext.UserName, @event);

            return Task.CompletedTask;
        }
    }
}