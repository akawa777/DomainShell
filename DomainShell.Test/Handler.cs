using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test
{
    public class OrderCompletedEventHandler : IDomainEventHandler<OrderCompletedEvent>, IDomainEventHandler<OrderCompletedExceptionEvent>
    {
        public OrderCompletedEventHandler(ISession session, IOrderRepository orderRepository, ICreditCardService creditCardService, IMailService mailService)
        {
            _session = session;
            _orderRepository = orderRepository;
            _creditCardService = creditCardService;
            _mailService = mailService;
        }

        private ISession _session;
        private IOrderRepository _orderRepository;      
        private ICreditCardService  _creditCardService;
        private IMailService _mailService;

        public void Handle(OrderCompletedEvent domainEvent)
        {
            try
            {
                using(_session.Open())
                {
                    OrderModel orderModel = _orderRepository.Find(domainEvent.OrderId);

                    orderModel.SendCompletedMail(_mailService);
                }
            }
            catch(Exception e)
            {
                _session.OnException(e);
                throw e;
            }
        }

        public void Handle(OrderCompletedExceptionEvent domainEvent)
        {
            try
            {
                using(var tran = _session.Tran())
                {
                    OrderModel orderModel = _orderRepository.Find(domainEvent.OrderId);

                    orderModel.CancelCompleted(_creditCardService);

                    _orderRepository.Commit(orderModel);
                    tran.Complete();
                }
            }
            catch(Exception e)
            {
                _session.OnException(e);
                throw e;
            }
        }
    }
}