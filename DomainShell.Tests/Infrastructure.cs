using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DomainShell.Infrastructure;

namespace DomainShell.Tests
{
    public class DomainEventBundle : IDomainEventBundle
    {
        public void Bundle(IDomainEventRegister register)
        {
            PersonWriteRepository writeRepository = new PersonWriteRepository();

            register.Set<PersonAddedEvent>(() => new PersonEventHandler(writeRepository));
            register.Set<PersonUpdatedEvent>(() => new PersonEventHandler(writeRepository));
            register.Set<PersonRemovedEvent>(() => new PersonEventHandler(writeRepository));
        }
    }

    public static class DataStore
    {
        static DataStore()
        {
            _personTable.Columns.Add("id", typeof(int));
            _personTable.Columns.Add("name", typeof(string));

            _personTable.Rows.Add(new object[] { 1, "1" });
            _personTable.Rows.Add(new object[] { 2, "2" });
            _personTable.Rows.Add(new object[] { 3, "3" });

            _personTable.AcceptChanges();
        }

        private static DataTable _personTable = new DataTable();

        public static DataTable PersonTable { get { return _personTable; } }
    }
}
