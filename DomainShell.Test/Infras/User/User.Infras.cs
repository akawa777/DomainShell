using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;
using DomainShell.Test.Domains;
using DomainShell.Test.Domains.User;

namespace DomainShell.Test.Infras.User
{    
    public class UserRepository : IUserRepository
    {
        public UserRepository(IConnection connection)
        {
            _connection = connection;
        }

        private IConnection _connection;

        public UserModel Find(string userId, bool throwError = false)
        {
            var readSet = Read(userId);

            UserModel userModel = Map(readSet).FirstOrDefault();

            if (throwError && userModel == null) throw new Exception("user not found.");

            return userModel;
        }

        private (IDataReader reader, IDbCommand command) Read(string userId)
        {
            IDbCommand command = _connection.CreateCommand();

            string sql = $@"
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

        private IEnumerable<UserModel> Map((IDataReader reader, IDbCommand command) readSet)
        {
            try
            {
                IDataReader reader = readSet.reader;

                while (reader.Read())
                {
                    var userProxyObject = new ProxyObject<UserModel>();

                    userProxyObject
                        .Set(m => m.UserId, (m, p) => reader[p.Name])
                        .Set(m => m.UserName, (m, p) => reader[p.Name])
                        .Set(m => m.RecordVersion, (m, p) => reader[p.Name]);

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
