using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain.Product
{
    public class ProductModel : IAggregateRoot
    {
        public ProductModel()
        {

        }

        public ProductModel(ProductProxy proxy)
        {
            ProductId = proxy.ProductId;
            ProductName = proxy.ProductName;
            Price = proxy.Price;

            State = State.Stored;
        }

        public State State { get; private set; }

        public void Stored()
        {
            State = State.Stored;
        }        

        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }   
}
