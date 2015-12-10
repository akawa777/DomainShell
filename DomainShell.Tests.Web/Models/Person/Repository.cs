using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using System.Linq.Expressions;

namespace DomainShell.Tests.Web.Models.Person
{ 
    public class PersonReadRepository
    {
        public Person Load(int id)
        {
            DataRow[] rows = DataStore.PersonTable.Data.Select(string.Format("id = {0}", id));

            if (rows.Length == 0)
            {
                return null;
            }

            Person person = new Person();

            person.Id = rows[0].Field<int>("id");
            person.Name = rows[0].Field<string>("name");

            return person;
        }

        public int GetNewId()
        {
            return DataStore.PersonTable.GetNewId();
        }
    }

    public class PersonWriteRepository
    {
        public void Add(Person person)
        {
            DataRow[] rows = DataStore.PersonTable.Data.Select(string.Format("id = {0}", person.Id));

            if (rows.Length > 0)
            {
                throw new Exception("not exist person");
            }

            DataRow row = DataStore.PersonTable.Data.NewRow();
            row["id"] = person.Id;
            row["name"] = person.Name;

            DataStore.PersonTable.Data.Rows.Add(row);

            DataStore.PersonTable.Data.AcceptChanges();
        }

        public void Update(Person person)
        {
            DataRow[] rows = DataStore.PersonTable.Data.Select(string.Format("id = {0}", person.Id));

            if (rows.Length == 0)
            {
                throw new Exception("not exist person");
            }

            rows[0]["name"] = person.Name;

            DataStore.PersonTable.Data.AcceptChanges();
        }

        public void Delete(Person person)
        {
            DataRow[] rows = DataStore.PersonTable.Data.Select(string.Format("id = {0}", person.Id));

            if (rows.Length == 0)
            {
                throw new Exception("not exist person");
            }

            DataStore.PersonTable.Data.Rows.Remove(rows[0]);

            DataStore.PersonTable.Data.AcceptChanges();
        }
    }

    public class PersonDataReadRepository
    {
        public PersonData Load(int id)
        {
            DataRow[] rows = DataStore.PersonTable.Data.Select(string.Format("id = {0}", id));

            if (rows.Length == 0)
            {
                return null;
            }

            PersonData person = new PersonData();

            person.Id = rows[0].Field<int>("id");
            person.Name = rows[0].Field<string>("name");

            return person;
        }

        public PersonData[] GetAll()
        {
            List<PersonData> persons = new List<PersonData>();

            foreach (DataRow row in DataStore.PersonTable.Data.Rows)
            {
                persons.Add(new PersonData { Id = row.Field<int>("id"), Name = row.Field<string>("name") });
            }

            return persons.OrderBy(x => x.Id).ToArray();
        }
    }
}
