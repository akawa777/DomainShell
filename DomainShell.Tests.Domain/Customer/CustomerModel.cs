﻿using System;
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

        public CustomerModel(CustomerRecord record)
        {
            CustomerId = record.CustomerId;
            CustomerName = record.CustomerName;
            Address = record.Address;
            CreditCardNo = record.CreditCardNo;
            CreditCardHolder = record.CreditCardHolder;
            CreditCardExpirationDate = record.CreditCardExpirationDate;
        }

        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
    }
}
