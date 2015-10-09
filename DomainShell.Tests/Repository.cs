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
    public static class DataStore 
    {
        static DataStore()
        {
            _personTable.Columns.Add("id", typeof(int));
            _personTable.Columns.Add("name", typeof(string));

            _personTable.Rows.Add(new object[] { 1, "1" });
            _personTable.Rows.Add(new object[] { 2, "2" });
            _personTable.Rows.Add(new object[] { 3, "3" });
        }

        private static DataTable _personTable = new DataTable();

        public static DataTable PersonTable { get { return _personTable; } }
    }

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

        public int GetNewId()
        {
            if (DataStore.PersonTable.Rows.Count == 0)
            {
                return 1;
            }

            DataRow[] rows = DataStore.PersonTable.Select("id = max(id)");

            return rows[0].Field<int>("id") + 1;
        }
    }

    public class PersonWriteRepository
    {
        public void Add(Person person)
        {
            DataRow[] rows = DataStore.PersonTable.Select(string.Format("id = {0}", person.Id));

            if (rows.Length > 0)
            {
                throw new Exception("not exist person");
            }

            DataRow row = DataStore.PersonTable.NewRow();
            row["id"] = person.Id;
            row["name"] = person.Name;

            DataStore.PersonTable.Rows.Add(row);

            DataStore.PersonTable.AcceptChanges();
        }

        public void Update(Person person)
        {
            DataRow[] rows = DataStore.PersonTable.Select(string.Format("id = {0}", person.Id));

            if (rows.Length == 0)
            {
                throw new Exception("not exist person");
            }

            rows[0]["name"] = person.Name;

            DataStore.PersonTable.AcceptChanges();
        }

        public void Delete(Person person)
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

    public class TransactionProcessor : ITransactionProcessor
    {
        private static object o = new object();

        public void Execute(Action saveAction)
        {
            lock(o)
            {
                saveAction();
            }
        }
    }
}
