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
        private Dictionary<IDomainEvent, dynamic> _callbackCache = new Dictionary<IDomainEvent, dynamic>();
        public void Callback<TResult>(IDomainEvent<TResult> message, Action<TResult> action)
        {
            _callbackCache[message] = action as dynamic;
        }

        Dictionary<IDomainEvent, dynamic> IDomainEventCallbackCache.GetCallbackCache()
        {
            return _callbackCache;
        }
    }

    public interface IAggregateRoot
    {
        EventList EventList { get; }
    }
}
