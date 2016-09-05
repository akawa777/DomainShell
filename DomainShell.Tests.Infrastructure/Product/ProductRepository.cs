using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Dagent;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Product;

namespace DomainShell.Tests.Infrastructure.Product
{
    public class ProductRepository
    {
        public ProductRepository(Session session)
        {
            _session = session;
        }

        private Session _session;

        private DbCommand CreateDbCommand()
        {
            return _session.GetPort<DbConnection>().CreateCommand();
        }

        public ProductModel Find(string productId)
        {
            DagentDatabase db = new DagentDatabase(_session.GetPort<DbConnection>());

            ProductModel model = db.Query<ProductModel>("Product", new { ProductId = productId }).Single();

            return model;
        }

        public List<ProductModel> GetAll()
        {
            DagentDatabase db = new DagentDatabase(_session.GetPort<DbConnection>());

            List<ProductModel> list = db.Query<ProductModel>(@"
                select * from Product order by ProductId        
            ").List();


            return list;
        }
    }   
}
