using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Domain.Models;  

namespace DomainShell.Tests.Domain.Service
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
