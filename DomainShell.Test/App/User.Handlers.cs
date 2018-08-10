using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Threading;
using System.Threading.Tasks;
using DomainShell.Test.Domain.UserAggregate;

namespace DomainShell.Test.Domain.OrderAggregate
{
    public class UserEventHandler : IDomainEventAsyncHandler<UserRegisterdEvent>
    {
        public void Handle(UserRegisterdEvent domainEvent)
        {
            Log.SetMessage($"{nameof(UserEventHandler)} {nameof(Handle)} {nameof(UserRegisterdEvent)}");
        }
    }
}