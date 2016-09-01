using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using System.Dynamic;

namespace DomainShell.Tests.Domain.Customer
{  
    public class CustomerModel : IAggregateRoot
    {
        public CustomerModel()
        {

        }

        public CustomerModel(CustomerProxy proxy)
        {
            CustomerId = proxy.CustomerId;
            CustomerName = proxy.CustomerName;
            Address = proxy.Address;
            CreditCardNo = proxy.CreditCardNo;
            CreditCardHolder = proxy.CreditCardHolder;
            CreditCardExpirationDate = proxy.CreditCardExpirationDate;

            State = State.Stored;
        }

        public State State { get; private set;  }

        public void Stored()
        {
            State = State.Stored;
        }

        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
    }
}
