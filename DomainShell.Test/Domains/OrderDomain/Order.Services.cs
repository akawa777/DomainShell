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

        public PaymentId Pay(Order order)
        {            
            var paymentId = Guid.NewGuid().ToString();

            return new PaymentId(paymentId);
        }

        public bool ExistsUser(Order order)
        {
            var user = _userRepository.Find(order.UserId);

            return user != null;
        }
    }
}