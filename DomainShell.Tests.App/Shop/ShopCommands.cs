using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.App.Shop
{
    public class CartAddItemCommand
    {
        public string CustomerId { get; set; }
        public string ProductId { get; set; }
        public int Number { get; set; }
    }

    public class CartAddItemResult
    {
        public CartAddItemResult()
        {
            Success = true;
            Messages = new List<string>();
        }

        public bool Success { get; set; }
        public List<string> Messages { get; set; }
        public string CartItemId { get; set; }
    }

    public class CartUpdateItemCommand
    {
        public string CustomerId { get; set; }
        public string CartItemId { get; set; }
        public int Number { get; set; }
    }

    public class CartUpdateItemResult
    {
        public CartUpdateItemResult()
        {
            Success = true;
            Messages = new List<string>();
        }

        public List<string> Messages { get; set; }
        public bool Success { get; set; }
    }

    public class CartRemoveItemCommand
    {
        public string CustomerId { get; set; }
        public string CartItemId { get; set; }
    }

    public class CartRemoveItemResult
    {
        public CartRemoveItemResult()
        {
            Success = true;
            Messages = new List<string>();
        }

        public List<string> Messages { get; set; }
        public bool Success { get; set; }
    }

    public class PaymentCommand
    {
        public string CustomerId { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
        public string ShippingAddress { get; set; }
    }

    public class PaymentResult
    {
        public PaymentResult()
        {
            Success = true;
            Messages = new List<string>();
        }

        public List<string> Messages { get; set; }
        public bool Success { get; set; }
    }
}
