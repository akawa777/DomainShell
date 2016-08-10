using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain.Cart;
using DomainShell.Tests.Domain.Product;
using DomainShell.Tests.Domain.Delivery;

namespace DomainShell.Tests.Domain.Payment
{
    public class PaymentModel : IAggregateRoot
    {
        public PaymentModel()
        {
            PaymentItemList = new List<PaymentItemModel>();
            PaymentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public PaymentModel(List<PaymentItemModel> paymentItemList)
            : this()
        {
            PaymentItemList = paymentItemList;
        }

        public string PaymentId { get; set; }
        public string PaymentDate { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Postage { get; set; }        

        public List<PaymentItemModel> PaymentItemList { get; set; }

        public DeliveryModel Delivery { get; set; }

        public void Pay(
            string creditCardNo, 
            string creditCardHolder, 
            string creditCardExpirationDate,            
            IPaymentService service)
        {
            CreditCardNo = creditCardNo;
            CreditCardHolder = creditCardHolder;
            CreditCardExpirationDate = creditCardExpirationDate;            

            service.Pay(this);

            Delivery = new DeliveryModel();

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
        public ProductModel Product { get; set; }
        public decimal PriceAtTime { get; set; }
        public int Number { get; set; }
    }
}
