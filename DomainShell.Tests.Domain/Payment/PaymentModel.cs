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
            State = new State();
        }

        public string PaymentId { get; set; }
        public string PaymentDate { get; set; }
        public string CustomerId { get; set; }
        public CreditCardValue CreditCard { get; set; }        
        public string ShippingAddress { get; set; }
        public decimal Postage { get; set; }
        public decimal Tax { get; set; }
        public decimal PaymentAmount { get; set; }

        public List<PaymentItemModel> PaymentItemList { get; set; }   

        public void Pay(
            CreditCardValue creditCard,    
            ICreditCardService creditCardService)
        {
            if (!string.IsNullOrEmpty(PaymentId))
            {
                throw new Exception("already paied.");
            }

            if (string.IsNullOrEmpty(ShippingAddress))
            {
                throw new Exception("ShippingAddress required.");
            }

            if (PaymentAmount == 0)
            {
                throw new Exception("PaymentAmount required.");
            }

            PaymentDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            CreditCard = creditCard;

            creditCardService.Pay(CreditCard, PaymentAmount);

            State.New();
        }        

        public State State { get; private set; }
    }

    public class PaymentItemModel
    {
        public string PaymentId { get; set; }
        public string PaymentItemId { get; set; }
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
