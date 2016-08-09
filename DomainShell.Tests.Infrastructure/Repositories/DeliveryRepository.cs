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
    public class DeliveryRepository : IRepositroy<DeliveryModel>
    {
        public DeliveryRepository(Session session)
        {
            _session = session;
        }

        private Session _session;  

        public DeliveryModel Get(string deliveryId)
        {
            return null;
        }

        public void Save(DeliveryModel delivery)
        {
            delivery.Accepted();
        }
    }
}
