using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain
{
    public class AddressValue : IValue
    {
        protected AddressValue()
        {
        }

        public AddressValue(string zipCode, string city)
        {
            ZipCode = zipCode;
            City = city;
        }

        public string ZipCode { get; protected set; }

        public string City { get; protected set; }

        public string Value
        {
            get { return string.Join(":", ZipCode, City); }
        }
    }
}
