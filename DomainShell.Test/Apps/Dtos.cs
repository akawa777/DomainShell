using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using DomainShell.Test.Domains;
using DomainShell.Test.Domains.UserDomain;
using DomainShell.Test.Domains.OrderDomain;

namespace DomainShell.Test.Apps
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public string OrderDate { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string CreditCardCode { get; set; }
        public string PayId { get; set; }        
        public int RecordVersion { get; set; }        
    }
}
