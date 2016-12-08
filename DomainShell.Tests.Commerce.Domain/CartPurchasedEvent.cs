using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Commerce.Domain
{
    public class CartCheckoutedEvent : IDomainEvent
    {
        public int CustomerId
        {
            get;
            set;
        }

        public CreditCardValue CreditCard
        {
            get;
            set;
        }

        public int PaymentId
        {
            get;
            set;
        }

        public decimal TotalPrice
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public List<PucharseDto> PucharseDtoList
        {
            get;
            set;
        }
    }
}
