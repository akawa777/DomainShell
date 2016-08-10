using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Delivery;

namespace DomainShell.Tests.Infrastructure.Delivery
{
    public class DeliveryReader
    {
        public DeliveryReader(Session session)
        {
            _session = session;
        }

        private Session _session;  

        public List<DeliveryReadModel> GetAllDelivery()
        {
            return new List<DeliveryReadModel>();
        }
    }
}
