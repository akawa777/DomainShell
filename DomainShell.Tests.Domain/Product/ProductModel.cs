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
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }

        public State State { get; private set; }

        public void Create()
        {
            State = State.Created;
        }

        public void Update()
        {
            State = State.Updated;
        }

        public void Delete()
        {
            State = State.Deleted;
        }

        public void Accepted()
        {
            State = State.UnChanged;
        }        
    }   
}
