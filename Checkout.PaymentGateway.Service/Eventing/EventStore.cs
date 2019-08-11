using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Service.Eventing
{
    /// <summary>
    /// This is a placeholder for storing events
    /// </summary>
    internal class EventStore<T> : IEventStore<T>
    {
        private readonly IDictionary<(string, Guid), IList<IEvent<T>>> _data = new ConcurrentDictionary<(string, Guid), IList<IEvent<T>>>();

        public async Task<IEnumerable<IEvent<T>>> Get(string userName, Guid id)
        {
            var key = (userName, id);
            if (_data.TryGetValue(key, out var items)) return items.ToList();

            return Enumerable.Empty<IEvent<T>>();
        }

        public void Add(string userName, IEvent<T> item)
        {
            var key = (userName, item.Id);
            if (_data.TryGetValue(key, out var items))
            {
                items.Add(item);
            }
            else
            {
                _data.Add(key, new List<IEvent<T>> { item });
            }

        }
    }
}