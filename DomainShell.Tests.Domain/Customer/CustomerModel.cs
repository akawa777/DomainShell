using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using System.Dynamic;

namespace DomainShell.Tests.Domain.Customer
{
    public class CustomerEntity
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
    }

    public class CustomerModel : IDomainModel<CustomerEntity>, IAggregateRoot
    {   
        public string CustomerId { get; private set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }

        void IDomainModel<CustomerEntity>.Map(CustomerEntity entity)
        {
            CustomerId = entity.CustomerId;
            CustomerName = entity.CustomerName;
            Address = entity.Address;
            CreditCardNo = entity.CreditCardNo;
            CreditCardHolder = entity.CreditCardHolder;
            CreditCardExpirationDate = entity.CreditCardExpirationDate;
        }
    }
}
