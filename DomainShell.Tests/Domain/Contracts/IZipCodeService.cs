using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain.Contracts
{
    public interface IZipCodeService : IDomainService
    {
        string GetCityName(string zipCode);
    }    
}
