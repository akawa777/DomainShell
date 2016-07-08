using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using DomainShell.Tests.Web.Infrastructure;
using DomainShell.Tests.Web.Models;

namespace DomainShell.Tests.Web.Repositories.Read
{ 
    public class PersonReadRepository
    {
        public Person Load(string id)
        {
            DataRow[] rows = DataStore.PersonTable.Select(string.Format("id = {0}", id));

            if (rows.Length == 0)
            {
                return null;
            }

            Person person = new Person();

            person.Id = rows[0].Field<string>("id");
            person.Name = rows[0].Field<string>("name");

            return person;
        }

        public Person[] GetAll()
        {
            List<Person> persons = new List<Person>();

            foreach (DataRow row in DataStore.PersonTable.Rows)
            {
                persons.Add(new Person { Id = row.Field<string>("id"), Name = row.Field<string>("name") });
            }

            return persons.OrderBy(x => x.Id).ToArray();
        }
    }
}
