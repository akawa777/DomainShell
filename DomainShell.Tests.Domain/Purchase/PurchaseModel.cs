﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain.Cart;

namespace DomainShell.Tests.Domain.Purchase
{
    public class PurchaseModel : IAggregateRoot
    {
        public PurchaseModel()
        {
            PurchaseDetailList = new ReadOnlyCollection<PurchaseDetailModel>(_purchaseDetailList);
        }

        public string PurchaseId { get; set; }
        public string PaymentDate { get; set; }
        public string CustomerId { get; set; }
        public CreditCardValue CreditCard { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Postage { get; set; }
        public decimal Tax { get; set; }
        public decimal PaymentAmount { get; set; }

        public ReadOnlyCollection<PurchaseDetailModel> PurchaseDetailList { get; set; }
        private List<PurchaseDetailModel> _purchaseDetailList = new List<PurchaseDetailModel>();

        public void AddDetail(PurchaseDetailModel detail)
        {
            if (PurchaseDetailList.Any(x => x == detail))
            {
                throw new Exception("already exist in PurchaseDetailList.");
            }

            if (PurchaseDetailList.Count == 0)
            {                
                detail.PurchaseDetailId = "1";
            }
            else
            {
                detail.PurchaseDetailId = (PurchaseDetailList.Max(x => int.Parse(x.PurchaseDetailId)) + 1).ToString();
            }

            detail.PurchaseId = PurchaseId;

            _purchaseDetailList.Add(detail);
        }

        public void Pay(CreditCardValue creditCard, ICreditCardService creditCardService)
        {
            if (!string.IsNullOrEmpty(PaymentDate))
            {
                throw new Exception("already paid.");
            }

            if (string.IsNullOrEmpty(CustomerId))
            {
                throw new Exception("CustomerId required.");
            }

            if (string.IsNullOrEmpty(ShippingAddress))
            {
                throw new Exception("ShippingAddress required.");
            }

            if (creditCard == null)
            {
                throw new Exception("creditCard required.");
            }

            if (PaymentAmount == 0)
            {
                throw new Exception("PaymentAmount required.");
            }

            CreditCard = creditCard;
            creditCardService.Pay(CreditCard, PaymentAmount);

            State = State.Accepted;
        }

        public State State { get; private set; }
    }

    public class PurchaseDetailModel
    {
        public string PurchaseId { get; set; }
        public string PurchaseDetailId { get; set; }
        public string ProductId { get; set; }
        public decimal PriceAtTime { get; set; }
        public int Number { get; set; }
    }

    public class CreditCardValue
    {
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
    }
}
