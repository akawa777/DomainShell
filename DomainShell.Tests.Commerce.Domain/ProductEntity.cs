using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Commerce.Domain
{
    public class ProductEntity : IAggregateRoot<int>
    {
        public IEnumerable<IDomainEvent> GetEvents()
        {
            return Enumerable.Empty<IDomainEvent>();
        }

        public void ClearEvents()
        {
            
        }

        public int Id
        {
            get;
            private set;
        }

        public string ProductName
        {
            get;
            set;
        }

        public decimal Price
        {
            get;
            set;
        }
    }
}
