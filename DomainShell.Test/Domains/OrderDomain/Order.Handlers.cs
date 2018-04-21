using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Threading;
using System.Threading.Tasks;
using DomainShell.Test.Domains.UserDomain;

namespace DomainShell.Test.Domains.OrderDomain
{
    public class OrderEventHandler : 
        IDomainEventHandler<OrderPaidEvent>,      
        IDomainEventHandler<OrderPaidExceptionEvent>
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

        public void Handle(OrderPaidExceptionEvent domainEvent)
        {            
            Log.MessageList.Add($"{nameof(OrderEventHandler)} {nameof(Handle)} {nameof(OrderPaidExceptionEvent)}");

            try
            {
                using(var tran = Session.Tran())
                {                    
                    var order = _orderRepository.Find(domainEvent.OrderId);

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
        IDomainEventHandler<OrderReadIssuedCertificateEvent>
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

                    order.CertificateIssueCount++;

                    order.Register(_orderService);

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