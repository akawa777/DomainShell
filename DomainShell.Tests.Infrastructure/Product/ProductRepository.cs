using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
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
            return _session.GetConnectionPort<DbConnection>().CreateCommand();
        }

        public ProductModel Find(string productId)
        {
            DbCommand command = CreateDbCommand();

            command.CommandText = @"
                select * from Product where ProductId = @ProductId
            ";

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@ProductId";
            param.Value = productId;
            command.Parameters.Add(param);

            ProductProxy proxy = new ProductProxy();

            using (DbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    proxy.ProductId = reader["ProductId"].ToString();
                    proxy.ProductName = reader["ProductName"].ToString();
                    proxy.Price = int.Parse(reader["Price"].ToString());
                }

                return proxy == null ? null : new ProductModel(proxy);
            }
        }

        public List<ProductModel> GetAll()
        {
            DbCommand command = CreateDbCommand();

            command.CommandText = @"
                select * from Product order by ProductId
            ";            

            using (DbDataReader reader = command.ExecuteReader())
            {
                List<ProductModel> list = new List<ProductModel>();
                while (reader.Read())
                {
                    ProductProxy proxy = new ProductProxy();
                    proxy.ProductId = reader["ProductId"].ToString();
                    proxy.ProductName = reader["ProductName"].ToString();
                    proxy.Price = int.Parse(reader["Price"].ToString());                    

                    list.Add(new ProductModel(proxy));
                }

                return list;
            }
        }
    }   
}
