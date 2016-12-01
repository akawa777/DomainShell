using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Commerce.Domain
{
    public class PurchaseConstructorParameters
    {
        
    }

    public class PurchaseCreationSpec : ICreationSpec<PurchaseEntity, PurchaseConstructorParameters>
    {
        public PurchaseCreationSpec(int customerId, CreditCardValue creditCard, List<PucharseDto> pucharseDtoList)
        {
            _customerId = customerId;
            _creditCard = creditCard;
            _pucharseDtoList = pucharseDtoList;
        }

        private int _customerId;
        private CreditCardValue _creditCard;
        private List<PucharseDto> _pucharseDtoList;

        public PurchaseConstructorParameters ConstructorParameters()
        {
            return new PurchaseConstructorParameters();
        }

        public void Satisfied(PurchaseEntity target)
        {
            
        }
    }
}
