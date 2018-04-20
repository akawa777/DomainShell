using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell.Test.Domains.OrderDomain
{
    public class OrderEventHandler : IDomainEventHandler<OrderCompletedEvent>, IDomainEventHandler<OrderCompletedExceptionEvent>, IDomainEventHandler<OrderCanceledEvent>, IDomainEventHandler<OrderSendedMailEvent>
    {
        public OrderEventHandler(
            IOrderRepository orderRepository, 
            IOrderService orderService,
            IOrderCanceledRepository orderCanceledRepository)
        {            
            _orderRepository = orderRepository;
            _orderService = orderService;
            _orderCanceledRepository = orderCanceledRepository;
        }
        
        private IOrderRepository _orderRepository;
        private IOrderService _orderService;
        private IOrderCanceledRepository _orderCanceledRepository;

        public void Handle(OrderCompletedEvent domainEvent)
        {
            Console.WriteLine($"{nameof(OrderEventHandler)} {nameof(Handle)} {nameof(OrderCompletedEvent)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            try
            {
                using(Session.Open())
                {
                    Order order = _orderRepository.Find(domainEvent.OrderId);

                    order.SendCompletedMail(_orderService);
                }
            }
            catch(Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        public void Handle(OrderCompletedExceptionEvent domainEvent)
        {
            Console.WriteLine($"{nameof(OrderEventHandler)} {nameof(Handle)} {nameof(OrderCompletedExceptionEvent)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine($"exception {domainEvent.Mode.GetException().Message}");

            try
            {
                using(var tran = Session.Tran())
                {
                    Order order = _orderRepository.Find(domainEvent.OrderId);

                    order.CancelCompleted(_orderService);

                    _orderRepository.Save(order);

                    tran.Complete();
                }
            }
            catch(Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        public void Handle(OrderCanceledEvent domainEvent)
        {
            Console.WriteLine($"{nameof(OrderEventHandler)} {nameof(Handle)} {nameof(OrderCanceledEvent)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            try
            {
                using (var tran = Session.Tran())
                {
                    OrderCanceledModel orderCanceledModel = OrderCanceledModel.NewCanceled(domainEvent.OrderId);

                    Map(domainEvent, orderCanceledModel);

                    orderCanceledModel.Register();

                    _orderCanceledRepository.Save(orderCanceledModel);

                    tran.Complete();
                }
            }
            catch (Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        public void Handle(OrderSendedMailEvent domainEvent)
        {
            Console.WriteLine($"{nameof(OrderEventHandler)} {nameof(Handle)} {nameof(OrderSendedMailEvent)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            try
            {
                Order order = _orderRepository.Find(domainEvent.OrderId);
                order.SendCompletedMail(_orderService);
            }
            catch (Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        private void Map(OrderCanceledEvent domainEvent, OrderCanceledModel model)
        {
            model.User = domainEvent.User;
            model.OrderDate = domainEvent.OrderDate;
            model.ProductName = domainEvent.ProductName;
            model.Price = domainEvent.Price;
            model.PayId = domainEvent.PayId;            
        }
    }
}