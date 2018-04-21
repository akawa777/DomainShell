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
        IDomainEventHandler<OrderPaidExceptionEvent>,
        IDomainEventHandler<OrderIssuedCertificateEvent>
    {
        public void Handle(OrderRegisterdEvent domainEvent)
        {
            Log.MessageList.Add($"{nameof(Handle)} {nameof(OrderRegisterdEvent)}");
        }

        public void Handle(OrderPaidExceptionEvent domainEvent)
        {
            Log.MessageList.Add($"{nameof(Handle)} {nameof(OrderPaidExceptionEvent)}");
        }

        public void Handle(OrderIssuedCertificateEvent domainEvent)
        {
            Log.MessageList.Add($"{nameof(Handle)} {nameof(OrderIssuedCertificateEvent)}");
        }
    }
}