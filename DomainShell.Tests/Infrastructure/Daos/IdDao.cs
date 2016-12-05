using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using Dagent;

namespace DomainShell.Tests.Infrastructure.Daos
{
    public class IdDao
    {
        public IdDao(System.Data.Common.DbConnection connection)
        {
            _connection = connection;
        }

        private System.Data.Common.DbConnection _connection;

        public string Generate(string tablenName)
        {
            string sql = @"
                insert into IdManege
                select @tableName, max(Id) + 1, @guid from IdManege where TableName = @tableName;

                select Id from IdManege where Guid = @guid
            ";

            string guid = Guid.NewGuid().ToString();

            DagentDatabase db = new DagentDatabase(_connection);

            string id = db.Query(sql, new { tableName = tablenName, guid = guid }).Scalar<string>();

            if (string.IsNullOrEmpty(id))
            {
                id = "1";
                db.ExequteNonQuery(@"
                        delete from IdManege where Guid = @guid;
                        insert into IdManege values(@tableName, @id, @guid)
                    ",
                    new Parameter("tableName", tablenName),
                    new Parameter("id", id),
                    new Parameter("guid", guid));
            }

            return id;
        }
    }
}
