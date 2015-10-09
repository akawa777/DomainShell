using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DomainShell.CQRS.Query;

namespace DomainShell.Tests
{
    public class PersonListQuery : IQuery<List<PersonData>>
    {
    }

    public class PersonListQueryHandler : IQueryHandler<PersonListQuery, List<PersonData>>
    {
        public List<PersonData> Handle(PersonListQuery query)
        {
            List<PersonData> persons = new List<PersonData>();

            foreach (DataRow row in DataStore.PersonTable.Rows)
            {
                persons.Add(new PersonData { Id = row.Field<int>("id"), Name = row.Field<string>("name") });
            }

            return persons;
        }
    }
}
