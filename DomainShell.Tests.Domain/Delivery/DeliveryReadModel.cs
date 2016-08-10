using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.Domain.Delivery
{
    public class DeliveryReadModel
    {
        public string PaymentId { get; set; }
        public string DeliveryId { get; set; }
        public string CustomerName { get; set; }
        public string TopProductName { get; set; }
        public string BeginDate { get; set; }
        public string CompleteDate { get; set; }
    }   
}
