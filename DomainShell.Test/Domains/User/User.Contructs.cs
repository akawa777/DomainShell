using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;

namespace DomainShell.Test.Domains.User
{
    public interface IUserRepository
    {
        UserModel Find(string userId, bool throwError = false);
    }
}