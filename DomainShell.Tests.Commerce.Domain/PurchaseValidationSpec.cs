using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;

namespace DomainShell.Tests.Commerce.Domain
{
    public class PurchaseValidationSpec : IValidationSpec<PurchaseEntity>
    {
        public bool Validate(PurchaseEntity target, out string[] errors)
        {
            errors = new string[0];

            return true;
        }
    }
}
