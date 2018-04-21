using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using DomainShell.Test.Domains.UserDomain;

namespace DomainShell.Test.Domains.OrderDomain
{    
    public class OrderService : IOrderService
    {
        public OrderService(            
            IUserRepository userRepository)
        {            
            _userRepository = userRepository;
        }
        
        private IUserRepository _userRepository;

        public PaymentResult Pay(Order order)
        {            
            return PayBy(order as dynamic);
        }

        public void CancelPayment(Order order)
        {            
            Log.MessageList.Add($"{nameof(OrderService)} {nameof(CancelPayment)} {nameof(Order)}");
        }

        private PaymentResult PayBy(Order order)
        {            
            var paymentId = Guid.NewGuid().ToString();

            return new PaymentResult(paymentId, (int)(order.Price * 0.01m));
        }

        public PaymentResult PayBy(SpecialOrder specialOrder)
        {            
            var paymentId = Guid.NewGuid().ToString();

            return new PaymentResult(paymentId, (int)(specialOrder.Price * 0.1m));
        }

        public bool ExistsUser(Order order)
        {
            var user = _userRepository.Find(order.UserId);

            return user != null;
        }
    }
}