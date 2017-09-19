using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test.Domains
{
    public class OrderEventHandler : IDomainEventHandler<OrderCompletedEvent>, IDomainEventHandler<OrderCompletedExceptionEvent>, IDomainEventHandler<OrderCanceledEvent>
    {
        public OrderEventHandler(
            IOrderRepository orderRepository, 
            ICreditCardService creditCardService, 
            IMailService mailService,
            IOrderCanceledRepository orderCanceledRepository)
        {            
            _orderRepository = orderRepository;
            _creditCardService = creditCardService;
            _mailService = mailService;
            _orderCanceledRepository = orderCanceledRepository;
        }
        
        private IOrderRepository _orderRepository;      
        private ICreditCardService  _creditCardService;
        private IMailService _mailService;
        private IOrderCanceledRepository _orderCanceledRepository;

        public void Handle(OrderCompletedEvent domainEvent)
        {
            Console.WriteLine($"{nameof(OrderEventHandler)} {nameof(Handle)} {nameof(OrderCompletedEvent)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            try
            {
                using(Session.Open())
                {
                    OrderModel orderModel = _orderRepository.Find(domainEvent.OrderId);

                    orderModel.SendCompletedMail(_mailService);
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
                    OrderModel orderModel = _orderRepository.Find(domainEvent.OrderId);

                    orderModel.CancelCompleted(_creditCardService);

                    _orderRepository.Save(orderModel);

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

        private void Map(OrderCanceledEvent domainEvent, OrderCanceledModel model)
        {
            model.ProductName = domainEvent.ProductName;
            model.Price = domainEvent.Price;
            model.PayId = domainEvent.PayId;
            model.LastUser = domainEvent.LastUser;
        }
    }
}