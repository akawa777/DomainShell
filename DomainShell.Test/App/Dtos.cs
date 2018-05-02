using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainShell.Test.App
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public string OrderDate { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string CreditCardCode { get; set; }
        public string PaymentId { get; set; }         
    }
}
