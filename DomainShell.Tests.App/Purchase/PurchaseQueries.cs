using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.App.Purchase
{
    public class PurchasesQuery
    {
        public string CustomerId { get; set; }
    }

    public class Purchase
    {
        public string PurchaseId { get; set; }
        public string PaymentDate { get; set; }
        public string CustomerId { get; set; }
        public string ShippingAddress { get; set; }
        public decimal PaymentAmount { get; set; }        
    }
}
