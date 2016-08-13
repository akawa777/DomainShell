using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain.Customer
{
    public class CustomerModel : IAggregateRoot
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }        

        public State State { get; private set; }        

        public void Accepted()
        {
            State = State.UnChanged;
        }
    }
}
