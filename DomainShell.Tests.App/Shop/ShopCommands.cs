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
}
