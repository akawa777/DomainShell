using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DomainShell.CQRS.Query;

namespace DomainShell.Tests.Web.Models.Person
{
    public class PersonListQuery : IQuery<PersonData[]>
    {
    }

    public class PersonQuery : IQuery<PersonData>
    {
        public int Id { get; set; }
    }

    public class PersonQueryHandler : 
        IQueryHandler<PersonListQuery, PersonData[]>,
        IQueryHandler<PersonQuery, PersonData>
    {
        public PersonQueryHandler(PersonDataReadRepository repository)
        {
            _repository = repository;
        }

        private PersonDataReadRepository _repository;

        public PersonData[] Handle(PersonListQuery query)
        {
            return _repository.GetAll();
        }

        public PersonData Handle(PersonQuery query)
        {
            return _repository.Load(query.Id);
        }
    }
}
