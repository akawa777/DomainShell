﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain.Cart;
using DomainShell.Tests.Domain.Product;

namespace DomainShell.Tests.Domain.Payment
{
    public class PaymentModel : IAggregateRoot
    {
        public string PaymentId { get; set; }
        public string PaymentDate { get; set; }
        public string CustomerId { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Postage { get; set; }        

        public List<PaymentItemModel> PaymentItemList { get; set; }   

        public void Pay(
            string creditCardNo, 
            string creditCardHolder, 
            string creditCardExpirationDate,    
            ICreditCardService<PaymentModel> service)
        {
            if (!string.IsNullOrEmpty(PaymentId))
            {
                throw new Exception("already paied.");
            }

            PaymentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            CreditCardNo = creditCardNo;
            CreditCardHolder = creditCardHolder;
            CreditCardExpirationDate = creditCardExpirationDate;

            service.Pay(this);            

            State = State.Created;
        }

        public decimal PaymentAmount()
        {
            return PaymentItemList.Sum(x => x.PriceAtTime) + Postage;
        }

        public State State { get; private set; }

        public void Accepted()
        {
            State = State.UnChanged;
        }
    }

    public class PaymentItemModel
    {
        public string PaymentId { get; set; }
        public string PaymentItemId { get; set; }
        public string ProductId { get; set; }
        public decimal PriceAtTime { get; set; }
        public int Number { get; set; }
    }
}
