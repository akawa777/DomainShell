using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.Domain.Product;
using DomainShell.Tests.Domain.Purchase;

namespace DomainShell.Tests.Domain.Cart
{
    public class CartRecord
    {
        public string CartId { get; set; }
        public string CustomerId { get; set; }
        public List<CartItemRecord> CartItemList { get; set; }
    }

    public class CartItemRecord
    {
        public string CartId { get; set; }
        public string CartItemId { get; set; }
        public string ProductId { get; set; }
        public ProductRecord Product { get; set; }
        public int Number { get; set; }
    }
}
