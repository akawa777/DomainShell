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

        public ProductModel Find(string productId)
        {
            DbCommand command = _session.CreateCommand();

            command.CommandText = @"
                select * from Product where ProductId = @ProductId
            ";

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@ProductId";
            param.Value = productId;
            command.Parameters.Add(param);

            ProductModel product = new ProductModel();

            using (DbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    product.ProductId = reader["ProductId"].ToString();
                    product.ProductName = reader["ProductName"].ToString();
                    product.Price = int.Parse(reader["Price"].ToString());
                }

                return product;
            }
        }

        public List<ProductModel> GetAll()
        {
            DbCommand command = _session.CreateCommand();

            command.CommandText = @"
                select * from Product order by ProductId
            ";            

            using (DbDataReader reader = command.ExecuteReader())
            {
                List<ProductModel> list = new List<ProductModel>();
                while (reader.Read())
                {
                    ProductModel product = new ProductModel();
                    product.ProductId = reader["ProductId"].ToString();
                    product.ProductName = reader["ProductName"].ToString();
                    product.Price = int.Parse(reader["Price"].ToString());                    

                    list.Add(product);
                }

                return list;
            }
        }
    }   
}
