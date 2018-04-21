using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;
using DomainShell.Test.Domains;
using DomainShell.Test.Domains.UserDomain;

namespace DomainShell.Test.Infras.UserInfra
{    
    public class UserRepository : IUserRepository
    {
        public UserRepository(IConnection connection)
        {
            _connection = connection;
        }

        private IConnection _connection;

        public UserRead Find(string userId)
        {
            var readSet = Read(userId);

            var user = Map(readSet).FirstOrDefault();

            return user;
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

        private IEnumerable<UserRead> Map((IDataReader reader, IDbCommand command) readSet)
        {
            try
            {
                var reader = readSet.reader;

                while (reader.Read())
                {
                    var userProxyObject = new ProxyObject<UserRead>();

                    userProxyObject
                        .Set(m => m.UserId, (m, p) => reader[p.Name])
                        .Set(m => m.UserName, (m, p) => reader[p.Name]);

                    yield return userProxyObject.Material;
                }
            }
            finally
            {
                readSet.reader.Dispose();
                readSet.command.Dispose();
            }
        }
    }
}
