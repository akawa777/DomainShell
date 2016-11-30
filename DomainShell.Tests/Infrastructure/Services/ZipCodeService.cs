using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Domain.Contracts;

namespace DomainShell.Tests.Infrastructure.Services
{
    public class ZipCodeService : IZipCodeService
    {
        public string GetCityName(string zipCode)
        {
            return "xxx";
        }
    }
}
