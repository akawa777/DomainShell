using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Dagent;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Common;

namespace DomainShell.Tests.Infrastructure.Common
{
    public class IdService :  IIdService
    {
        public IdService(Session session)
        {
            _session = session;
        }

        private Session _session;

        public string CreateId<TEntity>()
        {
            string tableName = typeof(TEntity).Name.Substring(0, typeof(TEntity).Name.LastIndexOf("Model") + 1);

            DagentDatabase db = new DagentDatabase(_session.GetPort<DbConnection>());

            string guid = Guid.NewGuid().ToString();

            db.ExequteNonQuery(@"
                    insert into IdManege 
                    select @TableName, ifnull(max(Id), 0) + 1, @Guid from IdManege where TableName = @TableName
                ", new Parameter("TableName", tableName), new Parameter("Guid", guid));

            string id = db.Query(@"                    
                    select Id from IdManege where TableName = @TableName and Guid = @Guid
                ", new Parameter("TableName", tableName), new Parameter("Guid", guid)).Scalar<string>();

            return id;
        }
    }
}
