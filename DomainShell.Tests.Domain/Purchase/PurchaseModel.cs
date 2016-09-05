using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain.Common;
using DomainShell.Tests.Domain.Cart;

namespace DomainShell.Tests.Domain.Purchase
{
    public class PurchaseModel : IAggregateRoot
    {
        private PurchaseModel()          
        {   
            PurchaseDetails = new ReadOnlyCollection<PurchaseDetailModel>(_purchaseDetailList);

            State = State.Added;
        }
        public PurchaseModel(IIdService idService) : this()
        {
            PurchaseId = idService.CreateId<PurchaseModel>();
        }        

        public PurchaseModel(PurchaseProxy proxy) : this()
        {
            PurchaseId = proxy.PurchaseId;
            PaymentDate = proxy.PaymentDate;
            CustomerId = proxy.CustomerId;
            CreditCardNo = proxy.CreditCardNo;
            CreditCardHolder = proxy.CreditCardHolder;
            CreditCardExpirationDate = proxy.CreditCardExpirationDate;
            ShippingAddress = proxy.ShippingAddress;
            Postage = proxy.Postage;
            Tax = proxy.Tax;
            PaymentAmount = proxy.PaymentAmount;

            foreach (PurchaseDetailProxy detailProxy in proxy.PurchaseDetailList)
            {
                _purchaseDetailList.Add(new PurchaseDetailModel(detailProxy));
            }
        }

        public State State { get; private set; }

        public void Stored()
        {
            State = State.Stored;
        }

        public string PurchaseId { get; set; }
        public string PaymentDate { get; set; }
        public string CustomerId { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
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

        public void Pay(ICreditCardService creditCardService)
        {   
            creditCardService.Pay(CreditCardNo, CreditCardHolder, CreditCardExpirationDate, PaymentAmount);
        }
    }

    public class PurchaseDetailModel
    {
        public PurchaseDetailModel()
        {

        }

        public PurchaseDetailModel(PurchaseDetailProxy proxy)
        {
            PurchaseId = proxy.PurchaseId;            
            PurchaseDetailId = proxy.PurchaseDetailId;
            ProductId = proxy.ProductId;
            PriceAtTime = proxy.PriceAtTime;
            Number = proxy.Number;
        }

        public string PurchaseId { get; set; }
        public string PurchaseDetailId { get; set; }
        public string ProductId { get; set; }
        public decimal PriceAtTime { get; set; }
        public int Number { get; set; }
    }
}
