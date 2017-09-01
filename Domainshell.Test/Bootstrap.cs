using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace DomainShell.Test
{
    public static class Bootstrap
    {
        public static Container Container { get; private set; }

        static Bootstrap()
        {
            Container container = new Container();        
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();             

            container.Register<IMemoryConnection, MemoryConnection>(Lifestyle.Scoped);
            container.Register<ISession, Session>(Lifestyle.Scoped);            

            container.Register<IOrderRepository, OrderRepository>(Lifestyle.Scoped);

            container.Register<IOrderValidator, OrderValidator>(Lifestyle.Scoped);
            container.Register<ICreditCardService, CreditCardService>(Lifestyle.Scoped);
            container.Register<IMailService, MailService>(Lifestyle.Scoped);

            container.Register<IDomainEventHandler<OrderCompletedEvent>, OrderCompletedEventHandler>(Lifestyle.Scoped);
            container.Register<IDomainEventHandler<OrderCompletedExceptionEvent>, OrderCompletedEventHandler>(Lifestyle.Scoped);

            container.Register<OrderCommandApp>(Lifestyle.Scoped);
            container.Register<OrderQueryApp>(Lifestyle.Scoped);            

            container.Verify();

            DomainEventFoundation.Startup(() => new InTranDomainEventScope(container), () => new OuterTranDomainEventScope(container));

            Container = container;
        }
    }

}