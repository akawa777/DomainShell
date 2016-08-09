using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.Domain.ReadModels
{
    public class CartReadModel
    {
        public string CartId { get; set; }
        public string MainProductName { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalNumber { get; set; }
    }

    public class CartDetailReadModel
    {
        public string CartId { get; set; }
        public string CartItemId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Number { get; set; }
    }
}
