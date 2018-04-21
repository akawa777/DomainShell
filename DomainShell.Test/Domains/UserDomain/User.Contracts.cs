using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;

namespace DomainShell.Test.Domains.UserDomain
{
    public interface IUserRepository
    {
        UserRead Find(string userId);
    }
}