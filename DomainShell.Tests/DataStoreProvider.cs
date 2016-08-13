using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;


namespace DomainShell.Tests
{
    internal class DataStoreProvider
    {
        private static string _db = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\", "test.db");
        private static string _connectionString = "data source=" + _db;

        private static bool _inited = false;

        public static DbConnection CreateConnection()
        {
            if (!_inited)
            {
                Init();

                _inited = true;
            }

            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SQLite");

            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = _connectionString;

            return connection;
        }

        public static DbDataAdapter CreateDataAdapter(DbCommand selectCommand)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SQLite");

            DbDataAdapter adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = selectCommand;

            DbCommandBuilder builder = factory.CreateCommandBuilder();
            builder.DataAdapter = adapter;

            return adapter;
        }

        private static void Init()
        {
            if (File.Exists(_db))
            {
                File.Delete(_db);
            }

            File.Create(_db).Close();

            SQLiteConnection.CreateFile(_db);

            using (DbConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                CreateCustomer(connection);
                CreateProduct(connection);
                CreateCart(connection);
                CreatePayment(connection);
            }
        }

        private static void CreateCustomer(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                        create table Customer(
                            CustomerId integer primary key,
                            CustomerName nvarchar(100)
                        )
                    ";

                var ret = command.ExecuteNonQuery();
            }

            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "insert into Customer (CustomerName) values (@name)";

                for (int i = 0; i < 10; i++)
                {
                    DbParameter parameter = command.CreateParameter();

                    parameter.ParameterName = "@name";
                    parameter.Value = "customer_" + (i + 1).ToString();

                    command.Parameters.Add(parameter);

                    var ret = command.ExecuteNonQuery();

                    command.Parameters.Clear();
                }
            }
        }

        private static void CreateProduct(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                        create table Product(
                            ProductId integer primary key,
                            ProductName nvarchar(100),
                            Price int
                        )
                    ";

                var ret = command.ExecuteNonQuery();
            }

            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "insert into Product (ProductName, Price) values (@name, @price)";

                for (int i = 0; i < 10; i++)
                {
                    DbParameter parameter = command.CreateParameter();

                    parameter.ParameterName = "@name";
                    parameter.Value = "product_" + (i + 1).ToString();

                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();

                    parameter.ParameterName = "@price";
                    parameter.Value = 100 * (i + 1);

                    command.Parameters.Add(parameter);

                    var ret = command.ExecuteNonQuery();

                    command.Parameters.Clear();
                }
            }
        }

        private static void CreateCart(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                        create table Cart(
                            CartId integer primary key,
                            CustomerId int
                        )
                    ";

                command.CommandText = @"
                        create table CartItem(
                            CartId integer primary key,
                            CartItemdId int,                            
                            ProductId int,
                            Number int
                        )
                    ";

                var ret = command.ExecuteNonQuery();
            }
        }

        private static void CreatePayment(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                        create table Payment(
                            PaymentId integer primary key,
                            CustomerId int,                            
                            CreditCardNo nvarchar(100),
                            CreditCardHolder nvarchar(100),
                            CreditCardExpirationDate nvarchar(100),
                            ShippingAddress nvarchar(100),
                            Postage int
                        )
                    ";

                command.CommandText = @"
                        create table PaymentItem(
                            PaymentId integer primary key,
                            PaymentItemdId int,                            
                            ProductId int,
                            PriceAtTime int,
                            Number int
                        )
                    ";

                var ret = command.ExecuteNonQuery();
            }
        }
    }
}
