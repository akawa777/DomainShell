using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.Domain.Payment
{
    public interface IPaymentService
    {        
        void Pay(PaymentModel payment);
    }

    public class PaymentService : IPaymentService
    {
        public void Pay(PaymentModel payment)
        {

        }
    } 
}
