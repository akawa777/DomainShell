using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Commerce.Domain
{
    public class CreditCardValue : IValue
    {
        public CreditCardValue(int cardCompanyId, int cardNo)
        {
            CardCompanyId = cardCompanyId;
            CardNo = cardNo;
        }

        public int CardCompanyId
        {
            get;
            protected set;
        }

        public int CardNo
        {
            get;
            protected set;
        }

        public string Value
        {
            get 
            {
                return string.Join(":", CardCompanyId, CardNo);
            }
        }
    }
}
