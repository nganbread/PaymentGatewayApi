using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Service.Eventing
{
    public interface IEventDispatcher<T>
    {
        Task Dispatch(IEvent<T> @event);
    }
}