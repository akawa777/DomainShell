using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.App.Shop
{
    public class Customer
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
    }

    public class Product
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }

    public class CartItem
    {
        public string CartId { get; set; }
        public string CartItemId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Number { get; set; }
    }

    public class PaymentAmountInfo
    {
        public decimal Postage { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PaymentAmount { get; set; }
    }

    public class PaymentContent
    {
        public string PaymentId { get; set; }
        public string PaymentDate { get; set; }
        public string CustomerId { get; set; }
        public string ShippingAddress { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}
