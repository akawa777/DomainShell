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
            if (cardCompanyId <= 0)
            {
                throw new ArgumentException("cardCompanyId is inivalid");
            }
            else if (cardNo <= 0)
            {
                throw new ArgumentException("cardNo is inivalid");
            }

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
