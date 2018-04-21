using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test.Domains.UserDomain
{
    public class UserRead : ReadAggregateRoot
    {
        protected UserRead()
        {
            
        }

        public string UserId { get; private set; }

        public string UserName { get; private set; }
    }
}