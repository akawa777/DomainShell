using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using DomainShell.Test.Domain.UserAggregate;

namespace DomainShell.Test.Infra.UserAggregate
{    
    public class UserRepository : IUserRepository
    {
        public UserRepository(IConnection connection)
        {
            _connection = connection;
        }

        private IConnection _connection;

        public User Find(string userId)
        {
            var readSet = Read(userId);

            var user = Map(readSet).FirstOrDefault();

            return user;
        }

        public void Save(User user)
        {
            if (!user.State.Modified()) return;

            if (user.Deleted)
            {
                Delete(user);
            }
            else if (string.IsNullOrEmpty(user.LastUpdate))
            {
                Insert(user);
            }
            else
            {
                Update(user);
            }
        }

        private (IDataReader reader, IDbCommand command) Read(string userId)
        {
            var command = _connection.CreateCommand();

            var sql = $@"
                select * from LoginUser
                where UserId = @userId
            ";

            command.CommandText = sql;

            var sqlParam = command.CreateParameter();
            sqlParam.ParameterName = $"@{nameof(userId)}";
            sqlParam.Value = userId == null ? DBNull.Value : userId as object;

            command.Parameters.Add(sqlParam);

            return (command.ExecuteReader(), command);
        }

        private IEnumerable<User> Map((IDataReader reader, IDbCommand command) readSet)
        {
            try
            {
                var reader = readSet.reader;

                while (reader.Read())
                {
                    var userProxyObject = new ProxyObject<User>();

                    userProxyObject
                        .Set(m => m.UserId, (m, p) => reader[p.Name])
                        .Set(m => m.UserName, (m, p) => reader[p.Name])
                        .Set(m => m.PaymentPoint, (m, p) => reader[p.Name])
                        .Set(m => m.LastUpdate, (m, p) => reader[p.Name]);

                    yield return userProxyObject.Material;
                }
            }
            finally
            {
                readSet.reader.Dispose();
                readSet.command.Dispose();
            }
        }

        private void Insert(User user)
        {
            var parameters = GetParameters(user);

            var sql = $@"
                insert into LoginUser (
                    {string.Join(", ", parameters.Select(x => x.Name))}
                ) values (
                    {string.Join(", ", parameters.Select(x => $"@{x.Name}"))}
                )
            ";

            using(var command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParams(command, parameters);
                command.ExecuteNonQuery();
            }
        }

        private void Update(User user)
        {
           var parameters = GetParameters(user);

           var sql = $@"
               update LoginUser 
               set
                   {string.Join(", ", parameters.Select(x => $"{x.Name} = @{x.Name}"))}
               where
                   UserId = @{nameof(user.UserId)}
           ";

           using(var command = _connection.CreateCommand())
           {
               command.CommandText = sql;
               AddParams(command, parameters);
               AddUserIdParam(command, user);
               command.ExecuteNonQuery();
           }
        }

        private void Delete(User user)
        {
            var sql = $@"
                delete from LoginUser                 
                where
                    UserId = @{nameof(user.UserId)}
            ";

            using(var command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddUserIdParam(command, user);
                command.ExecuteNonQuery();
            }
        } 

        private IEnumerable<(string Name, object Value)> GetParameters(User user)
        {
            var x = user;

            yield return (nameof(x.UserName), x.UserName);
            yield return (nameof(x.PaymentPoint), x.PaymentPoint);
            yield return (nameof(x.LastUpdate), DateTime.Now.ToString("yyyyMMddmmss"));
        }

        private void AddParams(IDbCommand command, IEnumerable<(string Name, object Value)> parameters)
        {
            foreach (var (Name, Value) in parameters)
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = $"@{Name}";
                sqlParam.Value = Value;

                command.Parameters.Add(sqlParam);
            }
        }

        private void AddUserIdParam(IDbCommand command, User user)
        {
            var sqlParam = command.CreateParameter();
            sqlParam.ParameterName = $"@{nameof(user.UserId)}";
            sqlParam.Value = user.UserId;

            command.Parameters.Add(sqlParam);
        } 
    }
}
