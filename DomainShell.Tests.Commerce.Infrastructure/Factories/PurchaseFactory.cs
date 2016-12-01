using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;
using DomainShell.Tests.Commerce.Infrastructure.Services;

namespace DomainShell.Tests.Commerce.Infrastructure.Factories
{
    public class PurchaseFactory : IPurchaseFactory
    {
        public PurchaseFactory(ISession session)
        {
            _session = session;
            _idProvider = new IdProvider(_session);
        }

        private ISession _session;
        private IdProvider _idProvider;

        public PurchaseEntity Create(ICreationSpec<PurchaseEntity, PurchaseConstructorParameters> spec)
        {
            PurchaseProxy purchase = new PurchaseProxy(_idProvider.Generate("Purchase"));

            spec.Satisfied(purchase);

            return purchase;
        }
    }
}
