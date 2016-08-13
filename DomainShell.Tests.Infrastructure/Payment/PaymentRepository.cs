using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Payment;

namespace DomainShell.Tests.Infrastructure.Payment
{
    public class PaymentRepository : IRepositroy<PaymentModel>
    {
        public PaymentRepository(Session session)
        {
            _session = session;
        }

        private Session _session;  

        public void Save(PaymentModel payment)
        {
            if (payment.State == State.Created)
            {
                Create(payment);
            }

            payment.Accepted();
        }

        private void Create(PaymentModel payment)
        {

        }
    }
}
