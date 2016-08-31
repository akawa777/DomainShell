using System;
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
            PurchaseDetails = new ReadOnlyCollection<PurchaseDetailModel>(_purchaseDetailList);
        } 

        public string PurchaseId { get; set; }
        public string PaymentDate { get; set; }
        public string CustomerId { get; set; }
        public CreditCardValue CreditCard { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Postage { get; set; }
        public decimal Tax { get; set; }
        public decimal PaymentAmount { get; set; }

        public ReadOnlyCollection<PurchaseDetailModel> PurchaseDetails { get; set; }
        private List<PurchaseDetailModel> _purchaseDetailList = new List<PurchaseDetailModel>();

        public void AddDetail(PurchaseDetailModel detail)
        {
            string purchaseDetailId;
            if (PurchaseDetails.Count == 0)
            {                
                purchaseDetailId = "1";
            }
            else
            {
                purchaseDetailId = (PurchaseDetails.Max(x => int.Parse(x.PurchaseDetailId)) + 1).ToString();
            }

            detail.PurchaseId = PurchaseId;
            detail.PurchaseDetailId = purchaseDetailId;

            _purchaseDetailList.Add(detail);
        }

        public void Pay(CreditCardValue creditCard, ICreditCardService creditCardService)
        {
            CreditCard = creditCard;
            creditCardService.Pay(CreditCard, PaymentAmount);
        }
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
