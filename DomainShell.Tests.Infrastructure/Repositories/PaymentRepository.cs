using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Models;

namespace DomainShell.Tests.Infrastructure.Repositries
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
            payment.Accepted();
        }
    }
}
