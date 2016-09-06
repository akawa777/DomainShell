using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.App.Shop
{
    public class AddCartItemCommand
    {
        public string CustomerId { get; set; }
        public string ProductId { get; set; }
        public int Number { get; set; }
    }

    public class AddCartItemResult
    {
        public AddCartItemResult()
        {
            Success = true;
            Messages = new List<string>();
        }

        public bool Success { get; set; }
        public List<string> Messages { get; set; }
        public string CartItemId { get; set; }
    }

    public class UpdateCartItemCommand
    {
        public string CustomerId { get; set; }
        public string CartItemId { get; set; }
        public int Number { get; set; }
    }

    public class UpdateCartItemResult
    {
        public UpdateCartItemResult()
        {
            Success = true;
            Messages = new List<string>();
        }

        public List<string> Messages { get; set; }
        public bool Success { get; set; }
    }

    public class RemoveCartItemCommand
    {
        public string CustomerId { get; set; }
        public string CartItemId { get; set; }
    }

    public class RemoveCartItemResult
    {
        public RemoveCartItemResult()
        {
            Success = true;
            Messages = new List<string>();
        }

        public List<string> Messages { get; set; }
        public bool Success { get; set; }
    }

    public class CheckoutCommand
    {
        public string CustomerId { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
        public string ShippingAddress { get; set; }
    }

    public class CheckoutResult
    {
        public CheckoutResult()
        {
            Success = true;
            Messages = new List<string>();
        }

        public List<string> Messages { get; set; }
        public bool Success { get; set; }
    }
}
