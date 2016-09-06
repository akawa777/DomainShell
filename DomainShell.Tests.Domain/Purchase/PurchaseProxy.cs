using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.Domain.Product;
using DomainShell.Tests.Domain.Cart;

namespace DomainShell.Tests.Domain.Purchase
{
    public class PurchaseProxy
    {
        public string PurchaseId { get; set; }
        public string PaymentDate { get; set; }
        public CustomerProxy Customer { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Postage { get; set; }
        public decimal Tax { get; set; }
        public decimal PaymentAmount { get; set; }

        public List<PurchaseDetailProxy> PurchaseDetailList { get; set; }
    }

    public class PurchaseDetailProxy
    {
        public string PurchaseId { get; set; }
        public string PurchaseDetailId { get; set; }
        public ProductProxy Product { get; set; }
        public decimal PriceAtTime { get; set; }
        public int Number { get; set; }
    }
}
