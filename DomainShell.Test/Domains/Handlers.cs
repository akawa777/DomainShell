using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test.Domains
{
    public class OrderEventHandler : IDomainEventHandler<OrderCompletedOutTranEvent>, IDomainEventHandler<OrderCompletedExceptionEvent>, IDomainEventHandler<OrderCanceledEvent>
    {
        public OrderEventHandler(
            IOrderRepository orderRepository, 
            ICreditCardService creditCardService, 
            IMailService mailService)
        {            
            _orderRepository = orderRepository;
            _creditCardService = creditCardService;
            _mailService = mailService;
        }
        
        private IOrderRepository _orderRepository;      
        private ICreditCardService  _creditCardService;
        private IMailService _mailService;
        private IUserRepository _userRepository;

        public void Handle(OrderCompletedOutTranEvent domainEvent)
        {
            Console.WriteLine($"{nameof(OrderEventHandler)} {nameof(Handle)} {nameof(OrderCompletedOutTranEvent)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");

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

            try
            {
                using(var tran = Session.Tran())
                {
                    OrderModel orderModel = _orderRepository.Find(domainEvent.OrderId);

                    orderModel.CancelCompleted(_creditCardService);                    

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

                    orderCanceledModel.Save();

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