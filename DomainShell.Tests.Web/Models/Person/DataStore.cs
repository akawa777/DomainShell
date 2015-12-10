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
    public static class DataStore 
    {
        static DataStore()
        {
            PersonTable = new PersonTable();
        }

        public static PersonTable PersonTable { get; set; }
    }

    public abstract class Table
    {        
        public abstract DataTable Data { get;set;}
        public abstract int GetNewId();
    }

    public class PersonTable : Table
    {
        public PersonTable()
        {
            Data = new DataTable();

            Data.Columns.Add("id", typeof(int));
            Data.Columns.Add("name", typeof(string));

            for (int i = 1; i < 3; i++)
            {
                Data.Rows.Add(new object[] { i.ToString(), "name_" + i.ToString() });
            }

            Data.AcceptChanges();

            _maxId = Data.Rows.Count;
        }

        public override DataTable Data { get; set; }

        private static int _maxId;

        public override int GetNewId()
        {
            var newId = _maxId++ + 1;

            return newId;
        }
    }
}
