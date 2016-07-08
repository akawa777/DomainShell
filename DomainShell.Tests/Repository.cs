using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Infrastructure;

namespace DomainShell.Tests
{ 
    public class PersonReadRepository
    {
        public Person Load(int id)
        {
            DataRow[] rows = DataStore.PersonTable.Select(string.Format("id = {0}", id));

            if (rows.Length == 0)
            {
                return null;
            }

            Person person = new Person();

            person.Id = rows[0].Field<int>("id");
            person.Name = rows[0].Field<string>("name");

            return person;
        }

        public Person[] GetAll()
        {
            List<Person> persons = new List<Person>();

            foreach (DataRow row in DataStore.PersonTable.Rows)
            {
                persons.Add(new Person { Id = row.Field<int>("id"), Name = row.Field<string>("name") });
            }

            return persons.OrderBy(x => x.Id).ToArray();
        }
    }

    public class PersonWriteRepository
    {
        private static object o = new object();

        public void Add(Person person)
        {
            lock (o)
            {
                DataRow[] rows = DataStore.PersonTable.Select("id = max(id)");

                person.Id = rows.Length == 0 ? 1 : int.Parse(rows[0]["id"].ToString()) + 1;

                DataRow row = DataStore.PersonTable.NewRow();
                row["id"] = person.Id;
                row["name"] = person.Name;

                DataStore.PersonTable.Rows.Add(row);

                DataStore.PersonTable.AcceptChanges();
            }
        }

        public void Update(Person person)
        {
            lock (o)
            {
                DataRow[] rows = DataStore.PersonTable.Select(string.Format("id = {0}", person.Id));

                if (rows.Length == 0)
                {
                    throw new Exception("not exist person");
                }

                rows[0]["name"] = person.Name;

                DataStore.PersonTable.AcceptChanges();
            }
        }

        public void Delete(Person person)
        {
            lock (o)
            {
                DataRow[] rows = DataStore.PersonTable.Select(string.Format("id = {0}", person.Id));

                if (rows.Length == 0)
                {
                    throw new Exception("not exist person");
                }

                DataStore.PersonTable.Rows.Remove(rows[0]);

                DataStore.PersonTable.AcceptChanges();
            }
        }
    }
}
