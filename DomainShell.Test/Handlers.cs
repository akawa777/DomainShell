using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test
{
    public class OrderCompletedEventHandler : IDomainEventHandler<OrderCompletedEvent>, IDomainEventHandler<OrderCompletedExceptionEvent>
    {
        public OrderCompletedEventHandler(IOrderRepository orderRepository, ICreditCardService creditCardService, IMailService mailService)
        {            
            _orderRepository = orderRepository;
            _creditCardService = creditCardService;
            _mailService = mailService;
        }
        
        private IOrderRepository _orderRepository;      
        private ICreditCardService  _creditCardService;
        private IMailService _mailService;

        public void Handle(OrderCompletedEvent domainEvent)
        {
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
            try
            {
                using(var tran = Session.Tran())
                {
                    OrderModel orderModel = _orderRepository.Find(domainEvent.OrderId);

                    orderModel.CancelCompleted(_creditCardService);

                    _orderRepository.Apply(orderModel);
                    tran.Complete();
                }
            }
            catch(Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }
    }
}