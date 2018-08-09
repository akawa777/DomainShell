using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Threading;
using System.Threading.Tasks;
using DomainShell.Test.Domain.UserAggregate;

namespace DomainShell.Test.Domain.OrderAggregate
{
    public class OrderEventHandler : 
        IDomainEventHandler<OrderPaidEvent>,      
        IDomainEventExceptionHandler<OrderPaidEvent>
    {
        public OrderEventHandler(            
            IOrderRepository orderRepository,
            IOrderService orderService,
            IUserRepository userRepository)
        {        
            _orderRepository = orderRepository;
            _orderService = orderService;    
            _userRepository = userRepository;
        }
        
        private IOrderRepository _orderRepository;
        private IOrderService _orderService;
        private IUserRepository _userRepository;

        public void Handle(OrderPaidEvent domainEvent)
        {
            Log.MessageList.Add($"{nameof(OrderEventHandler)} {nameof(Handle)} {nameof(OrderPaidEvent)}");

            try
            {
                using(var tran = Session.Tran())
                { 
                    var user = _userRepository.Find(domainEvent.UserId);

                    if (user == null) throw new Exception("user not found.");

                    user.PaymentPoint += domainEvent.PaymentPoint;

                    user.Register();

                    _userRepository.Save(user);

                    tran.Complete();
                }
            }
            catch(Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        public void Handle(OrderPaidEvent domainEvent, Exception exception)
        {            
            Log.MessageList.Add($"{nameof(OrderEventHandler)} {nameof(Handle)} {nameof(OrderPaidEvent)}");

            try
            {
                using(var tran = Session.Tran())
                {
                    Order order;

                    if (domainEvent.OrderId == 0)
                    {
                        order = _orderRepository.GetLastByUser(domainEvent.UserId);
                    }
                    else
                    {
                        order = _orderRepository.Find(domainEvent.OrderId);
                    }

                    if (order == null) throw new Exception("order not found.");

                    order.CancelPayment(_orderService);

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
    }

    public class OrderReadEventHandler :         
        IDomainEventAsyncHandler<OrderReadIssuedCertificateEvent>
    {
        public OrderReadEventHandler(
            IOrderRepository orderRepository,
            IOrderService orderService)
        {
            _orderRepository = orderRepository;
            _orderService = orderService;
        }

        private IOrderRepository _orderRepository;
        private IOrderService _orderService;

        public void Handle(OrderReadIssuedCertificateEvent domainEvent)
        {
            Log.MessageList.Add($"{nameof(OrderReadEventHandler)} {nameof(Handle)} {nameof(OrderReadIssuedCertificateEvent)}");

            try
            {
                using(var tran = Session.Tran())
                {                    
                    var order = _orderRepository.Find(domainEvent.OrderId);

                    if (order == null) throw new Exception("order not found.");                    

                    order.IncrementCertificateIssueCount(_orderService);

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
    }
}