using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell.Test.Domains.OrderDomain
{
    public class OrderEventHandler : 
        IDomainEventHandler<OrderRegisterdEvent>,      
        IDomainEventHandler<OrderPaidExceptionEvent>
    {
        public void Handle(OrderRegisterdEvent domainEvent)
        {
            Log.MessageList.Add($"{nameof(Handle)} {nameof(OrderRegisterdEvent)}");
        }

        public void Handle(OrderPaidExceptionEvent domainEvent)
        {
            var exception = domainEvent.Mode.GetException();
            Log.MessageList.Add($"{nameof(Handle)} {nameof(OrderPaidExceptionEvent)}");
        }
    }

    public class OrderReadEventHandler :         
        IDomainEventHandler<OrderReadIssuedCertificateEvent>
    {
        public void Handle(OrderReadIssuedCertificateEvent domainEvent)
        {
            Log.MessageList.Add($"{nameof(Handle)} {nameof(OrderReadIssuedCertificateEvent)}");
        }
    }
}