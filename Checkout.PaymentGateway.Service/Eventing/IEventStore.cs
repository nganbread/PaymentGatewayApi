using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Service.Eventing
{
    internal interface IEventStore<T>
    {
        Task<IEnumerable<IEvent<T>>> Get(string userName, Guid id);
        void Add(string userName, IEvent<T> item);
    }
}