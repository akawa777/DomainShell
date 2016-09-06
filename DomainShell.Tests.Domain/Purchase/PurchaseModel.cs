using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain.Common;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.Domain.Product;
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
            Customer = new CustomerModel(proxy.Customer);
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
        public string CustomerId { get { return Customer.CustomerId; } }
        public CustomerModel Customer { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Postage { get; set; }
        public decimal Tax { get; set; }
        public decimal PaymentAmount { get; set; }

        public ReadOnlyCollection<PurchaseDetailModel> PurchaseDetails { get; set; }
        private List<PurchaseDetailModel> _purchaseDetailList = new List<PurchaseDetailModel>();

        public PurchaseDetailModel CreateDetail()
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

            PurchaseDetailModel detail = new PurchaseDetailModel(PurchaseId, purchaseDetailId);

            _purchaseDetailList.Add(detail);

            return detail;
        }
    }

    public class PurchaseDetailModel
    {
        private PurchaseDetailModel()
        {

        }

        public PurchaseDetailModel(string purchaseId, string purchaseDetailId)
        {
            PurchaseId = purchaseId;
            PurchaseDetailId = purchaseDetailId;
        }

        public PurchaseDetailModel(PurchaseDetailProxy proxy)
        {
            PurchaseId = proxy.PurchaseId;            
            PurchaseDetailId = proxy.PurchaseDetailId;
            Product = new ProductModel(proxy.Product);
            PriceAtTime = proxy.PriceAtTime;
            Number = proxy.Number;
        }

        public string PurchaseId { get; private set; }
        public string PurchaseDetailId { get; private set; }
        public string ProductId { get { return Product.ProductId; } }
        public ProductModel Product { get; set; }
        public decimal PriceAtTime { get; set; }
        public int Number { get; set; }
    }
}
