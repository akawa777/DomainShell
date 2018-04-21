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
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

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

                    user.AddPaymentPoint(domainEvent.PaymentPoint);

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
        }
    }

    public class OrderReadEventHandler :         
        IDomainEventHandler<OrderReadIssuedCertificateEvent>
    {
        public void Handle(OrderReadIssuedCertificateEvent domainEvent)
        {
            Log.MessageList.Add($"{nameof(OrderReadEventHandler)} {nameof(Handle)} {nameof(OrderReadIssuedCertificateEvent)}");
        }
    }
}