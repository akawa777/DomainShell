using System;
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
        public decimal PaymentAmount { get; set; }

        private List<PaymentItemModel> PaymentItemList { get; set; }

        public PaymentItemModel[] PaymentItems()
        {
            return PaymentItemList.ToArray();
        }

        public void Add(CartModel cart)
        {
            decimal paymentAmount = 0m;
            foreach (CartItemModel item in cart.CartItems)
            {
                PaymentItemList.Add(new PaymentItemModel
                {
                    Product = item.Product,
                    Number = item.Number,
                    PriceAtTime = item.Product.Price
                });

                paymentAmount += item.Product.Price;
            }

            PaymentAmount += paymentAmount;

            cart.Delete();
        }

        public void Pay(
            string creditCardNo, 
            string creditCardHolder, 
            string creditCardExpirationDate,
            string shippingAddress,
            decimal postage,
            IPaymentService service)
        {
            CreditCardNo = creditCardNo;
            CreditCardHolder = creditCardHolder;
            CreditCardExpirationDate = creditCardExpirationDate;
            ShippingAddress = shippingAddress;
            Postage = postage;

            service.Pay(this);

            State = State.Created;
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
