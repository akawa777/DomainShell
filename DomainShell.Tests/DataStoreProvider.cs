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
                CreatePurchase(connection);
                CreateIdManege(connection);
            }
        }

        private static void CreateCustomer(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                        create table Customer(
                            CustomerId integer primary key,
                            CustomerName nvarchar(100),
                            Address nvarchar(100),
                            CreditCardNo nvarchar(100),
                            CreditCardHolder nvarchar(100),
                            CreditCardExpirationDate nvarchar(100)
                        )
                    ";

                var ret = command.ExecuteNonQuery();
            }

            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "insert into Customer (CustomerName, Address, CreditCardNo, CreditCardHolder, CreditCardExpirationDate) values (@name, @address, @creditCardNo, @creditCardHolder, @creditCardExpirationDate)";

                for (int i = 0; i < 10; i++)
                {
                    DbParameter parameter = command.CreateParameter();
                    parameter.ParameterName = "@name";
                    parameter.Value = "customer_" + (i + 1).ToString();
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@address";
                    parameter.Value = "address_" + (i + 1).ToString();
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@creditCardNo";
                    parameter.Value = "creditCardNo_" + (i + 1).ToString();
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@creditCardHolder";
                    parameter.Value = "creditCardHolder_" + (i + 1).ToString();
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@creditCardExpirationDate";
                    parameter.Value = "creditCardExpirationDate_" + (i + 1).ToString();
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
                            CartId integer,
                            CustomerId int,
                            primary key (CartId)
                        )
                    ";

                command.ExecuteNonQuery();

                command.CommandText = @"
                        create table CartItem(
                            CartId int,
                            CartItemId int,                            
                            ProductId int,
                            Number int,
                            primary key (CartId, CartItemId)
                        )
                    ";

                command.ExecuteNonQuery();
            }
        }

        private static void CreatePurchase(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                        create table Purchase(
                            PurchaseId integer,
                            PaymentDate nvarchar(100),
                            CustomerId int,                            
                            CreditCardNo nvarchar(100),
                            CreditCardHolder nvarchar(100),
                            CreditCardExpirationDate nvarchar(8),
                            ShippingAddress nvarchar(100),
                            Postage int,
                            Tax int,
                            PaymentAmount int,
                            primary key (PurchaseId)
                        )
                    ";

                command.ExecuteNonQuery();

                command.CommandText = @"
                        create table PurchaseDetail(
                            PurchaseId int,
                            PurchaseDetailId int,                            
                            ProductId int,
                            PriceAtTime int,
                            Number int,
                            primary key (PurchaseId, PurchaseDetailId)
                        )
                    ";

                command.ExecuteNonQuery();
            }
        }

        private static void CreateIdManege(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                        create table IdManege(
                            TableName nvarchar(100),
                            Id integer,
                            Guid nvarchar(100),
                            primary key (TableName, Id)
                        )
                    ";

                command.ExecuteNonQuery();
            }
        }
    }
}
