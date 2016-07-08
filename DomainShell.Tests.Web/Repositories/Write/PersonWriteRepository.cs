using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using DomainShell.Tests.Web.Infrastructure;
using DomainShell.Tests.Web.Models;

namespace DomainShell.Tests.Web.Repositories.Write
{ 
    public class PersonWriteRepository
    {
        private static object o = new object();

        public void Add(Person person)
        {
            lock (o)
            {
                DataRow[] rows = DataStore.PersonTable.Select("id = max(id)");

                int id = rows.Length == 0 ? 1 : int.Parse(rows[0]["id"].ToString()) + 1;

                person.Id = id.ToString();

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

        public void Update(Person person, Tran tran)
        {
            lock (o)
            {
                DataRow[] rows = DataStore.PersonTable.Select(string.Format("id = {0}", person.Id));

                if (rows.Length == 0)
                {
                    throw new Exception("not exist person");
                }

                rows[0]["name"] = person.Name;
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
