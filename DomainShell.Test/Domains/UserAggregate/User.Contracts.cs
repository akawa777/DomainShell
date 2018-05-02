using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainShell.Test.Domain.UserAggregate
{
    public interface IUserRepository
    {
        User Find(string userId);
        void Save(User user);
    }
}