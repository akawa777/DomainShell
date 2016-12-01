using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;

namespace DomainShell.Tests.Commerce.Domain.Handlers
{
    public class CartEventHandler : IDomianEventHandler<CartPurchasedEvent>
    {
        public CartEventHandler(IPurchaseFactory purchaseFactory, IPurchaseRepository purchaseRepository)
        {
            _purchaseFactory = purchaseFactory;
            _purchaseRepository = purchaseRepository;
        }

        private IPurchaseFactory _purchaseFactory;
        private IPurchaseRepository _purchaseRepository;        
             
        public void Handle(CartPurchasedEvent domainEvent)
        {
            PurchaseCreationSpec spec = new PurchaseCreationSpec(domainEvent.CustomerId, domainEvent.CreditCard, domainEvent.PucharseDtoList);

            PurchaseEntity purchase = _purchaseFactory.Create(spec);

            _purchaseRepository.Save(purchase);
        }
    }
}
