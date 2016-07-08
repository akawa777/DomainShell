using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DomainShell.Infrastructure;
using DomainShell.Tests.Web.Repositories.Write;
using DomainShell.Tests.Web.Events;

namespace DomainShell.Tests.Web.Infrastructure
{
    public class DomainEventBundle : IDomainEventBundle
    {
        public void Bundle(IDomainEventRegister register)
        {
            register.Set<PersonAddedEvent>(() => new PersonEventHandler());
            register.Set<PersonUpdatedEvent>(() => new PersonEventHandler());
            register.Set<PersonRemovedEvent>(() => new PersonEventHandler());
        }
    }

    public static class DataStore
    {
        static DataStore()
        {
            _personTable.Columns.Add("id", typeof(string));
            _personTable.Columns.Add("name", typeof(string));

            _personTable.Rows.Add(new object[] { "1", "1_name" });
            _personTable.Rows.Add(new object[] { "2", "2_name" });
            _personTable.Rows.Add(new object[] { "3", "3_name" });

            _personTable.AcceptChanges();
        }

        private static DataTable _personTable = new DataTable();

        public static DataTable PersonTable { get { return _personTable; } }
    }
}
