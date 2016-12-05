using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain.Contracts
{
    public interface IPersonRepository :
        IReadReposiory<PersonEntity, PersonId>, 
        IReadSpecReposiory<PersonEntity, PersonPredicate>,  
        IWriteRepository<PersonEntity>
    {
    }
}
