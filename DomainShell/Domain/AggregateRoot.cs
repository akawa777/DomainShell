using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Event;

namespace DomainShell.Domain
{
    public class EventList : List<IDomainEvent>, IDomainEventCallbackCache
    {
        private Dictionary<IDomainEvent, EventCallbackBase> _callbackCache = new Dictionary<IDomainEvent, EventCallbackBase>();
        public void Callback<TResult>(IDomainEvent<TResult> message, Action<TResult> action)
        {
            _callbackCache[message] = new EventCallback<TResult> { Callback = action };
        }

        Dictionary<IDomainEvent, EventCallbackBase> IDomainEventCallbackCache.GetCallbackCache()
        {
            return _callbackCache;
        }
    }

    public interface IAggregateRoot
    {
        EventList EventList { get; }
    }
}
