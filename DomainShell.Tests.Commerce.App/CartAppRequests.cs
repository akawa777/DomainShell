using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.Commerce.App
{
    public class CartItemListRequest
    {
        public int CustomerId { get; set; }
    }

    public class CartItemAddRequest
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemRemoveRequest
    {
        public int CustomerId { get; set; }
        public int CartItemNo { get; set; }
    }

    public class CartPurchaseRequest
    {
        public int CustomerId { get; set; }
        public int CardCompanyId { get; set; }
        public int CardNo { get; set; }
    }
}
