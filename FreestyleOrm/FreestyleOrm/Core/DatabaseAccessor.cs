using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;

namespace FreestyleOrm.Core
{
    internal interface IDatabaseAccessor
    {
        int Insert(Row row, QueryOptions queryOptions, out object lastId);
        int Update(Row row, QueryOptions queryOptions);
        int Delete(Row row, QueryOptions queryOptions);        
        IDataReader CreateTableReader(QueryOptions queryOptions, MapOptions mapOptions, out string[] primaryKeys);
        IDataReader CreateFetchReader(QueryOptions queryOptions);
    }

    internal enum ParameterFilter
    {
        All,
        PrimaryKeys,
        WithoutPrimaryKeys,
        RowVersion
    }

    internal class DatabaseAccessor : IDatabaseAccessor
    {
        public IDataReader CreateTableReader(QueryOptions queryOptions, MapOptions mapOptions, out string[] primaryKeys)
        {
            primaryKeys = GetPrimaryKeys(queryOptions, mapOptions);

            IDbCommand command = queryOptions.Connection.CreateCommand();
            command.Transaction = queryOptions.Transaction;

            string sql = $@"
                select * from {mapOptions.Table} where 1 = 0
            ";

            command.CommandText = sql;

            return command.ExecuteReader();
        }

        private string[] GetPrimaryKeys(QueryOptions queryOptions, MapOptions mapOptions)
        {
            IDbCommand command = queryOptions.Connection.CreateCommand();
            command.Transaction = queryOptions.Transaction;

            string table = mapOptions.Table.Split('.').Length == 1 ? mapOptions.Table.Split('.')[0] : mapOptions.Table.Split('.')[1];
            string schema = mapOptions.Table.Split('.').Length == 1 ? string.Empty : $"AND TABLE_SCHEMA = '{mapOptions.Table.Split('.')[0]}'";

            string sql = $@"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)), 'IsPrimaryKey') = 1
                AND TABLE_NAME = '{table}' {schema}
            ";

            command.CommandText = sql;

            List<string> columns = new List<string>();

            using (var reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    columns.Add(reader["COLUMN_NAME"].ToString());
                }

                reader.Close();
            }

            return columns.ToArray();
        }

        public int Insert(Row row, QueryOptions queryOptions, out object lastId)
        {
            lastId = null;

            IDbCommand command = queryOptions.Connection.CreateCommand();
            command.Transaction = queryOptions.Transaction;

            Dictionary<string, IDbDataParameter> parameters = GetParameters(row, command, row.AutoId ? ParameterFilter.WithoutPrimaryKeys : ParameterFilter.All);

            if (!string.IsNullOrEmpty(row.RelationIdColumn)
                && (
                    row[row.RelationIdColumn] == DBNull.Value
                    || row[row.RelationIdColumn] == null
                    || row[row.RelationIdColumn].ToString() == string.Empty
                    || (decimal.TryParse(row[row.RelationIdColumn].ToString(), out decimal result) && result == 0)
                )
                && _lastIdMap.TryGetValue(row.RelationEntityPath, out object id))
            {
                parameters[row.RelationIdColumn].Value = id;
            }

            string columnNames = string.Join(", ", parameters.Keys);
            string paramNames = string.Join(", ", parameters.Values.Select(x => x.ParameterName));

            string sql = $@"
                insert into {row.Table} ({columnNames}) values({paramNames})
            ";

            command.CommandText = sql;
            foreach (var param in parameters.Values) command.Parameters.Add(param);

            int rtn = command.ExecuteNonQuery();

            if (row.AutoId)
            {
                lastId = GetLastId(row, queryOptions);
                _lastIdMap[row.ExpressionPath] = lastId;
            }

            return rtn;
        }

        public int Update(Row row, QueryOptions queryOptions)
        {
            IDbCommand command = queryOptions.Connection.CreateCommand();
            command.Transaction = queryOptions.Transaction;

            Dictionary<string, IDbDataParameter> parameters = GetParameters(row, command, ParameterFilter.WithoutPrimaryKeys);
            Dictionary<string, IDbDataParameter> primaryKeyParameters = GetParameters(row, command, ParameterFilter.PrimaryKeys);
            Dictionary<string, IDbDataParameter> rowVersionParameters = GetParameters(row, command, ParameterFilter.RowVersion);
            
            IEnumerable<KeyValuePair<string, IDbDataParameter>> whereParameters = primaryKeyParameters.Concat(rowVersionParameters);

            string set = string.Join(", ", parameters.Select(x => $"{x.Key} = {x.Value.ParameterName}"));
            string where = string.Join(" and ", whereParameters.Select(x => $"{x.Key} = {x.Value.ParameterName}"));

            string sql = $@"
                update {row.Table} set {set} where {where}
            ";

            command.CommandText = sql;
            foreach (var param in parameters.Values) command.Parameters.Add(param);
            foreach (var param in whereParameters.Select(x => x.Value)) command.Parameters.Add(param);

            int rtn = command.ExecuteNonQuery();

            if (!string.IsNullOrEmpty(row.RowVersionColumn) && row.IsRootRow && rtn == 0) throw new InvalidOperationException("Concurrency is invalid.");

            return rtn;
        }

        public int Delete(Row row, QueryOptions queryOptions)
        {
            IDbCommand command = queryOptions.Connection.CreateCommand();
            command.Transaction = queryOptions.Transaction;

            Dictionary<string, IDbDataParameter> primaryKeyParameters = GetParameters(row, command, ParameterFilter.PrimaryKeys);
            Dictionary<string, IDbDataParameter> rowVersionParameters = GetParameters(row, command, ParameterFilter.RowVersion);

            IEnumerable<KeyValuePair<string, IDbDataParameter>> whereParameters = primaryKeyParameters.Concat(rowVersionParameters);

            string where = string.Join(" and ", whereParameters.Select(x => $"{x.Key} = {x.Value.ParameterName}"));

            string sql = $@"
                delete from {row.Table} where {where}
            ";

            command.CommandText = sql;
            foreach (var param in whereParameters.Select(x => x.Value)) command.Parameters.Add(param);

            int rtn = command.ExecuteNonQuery();

            if (!string.IsNullOrEmpty(row.RowVersionColumn) && row.IsRootRow && rtn == 0) throw new InvalidOperationException("Concurrency is invalid.");

            return rtn;
        }

        private Dictionary<string, IDbDataParameter> GetParameters(Row row, IDbCommand command, ParameterFilter parameterFilter)
        {
            Dictionary<string, IDbDataParameter> parameters = new Dictionary<string, IDbDataParameter>();

            foreach (var column in row.Columns)
            {
                bool isTargetColumn = false;

                if (parameterFilter == ParameterFilter.All)
                {
                    isTargetColumn = true;
                }
                else if (parameterFilter == ParameterFilter.PrimaryKeys)
                {
                    if (row.IsPrimaryKey(column) || row.IsRowVersionColumn(column)) isTargetColumn = true;
                }
                else if (parameterFilter == ParameterFilter.WithoutPrimaryKeys)
                {
                    if (!row.IsPrimaryKey(column)) isTargetColumn = true;
                }

                if (!isTargetColumn) continue;

                IDbDataParameter parameter;

                if (parameterFilter != ParameterFilter.PrimaryKeys
                    && !string.IsNullOrEmpty(row.RowVersionColumn)
                    && row.NewRowVersion != null
                    && column == row.RowVersionColumn)
                {
                    parameter = CreateParameter(command, $"new_{column}", row.NewRowVersion);
                }
                else
                {
                    parameter = CreateParameter(command, column, row[column]);
                }

                parameters[column] = parameter;
            }

            return parameters;
        }

        public IDbDataParameter CreateParameter(IDbCommand command, string name, object value)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = name.StartsWith("@") ? name : $"@{name}";
            parameter.Value = value;

            return parameter;
        }

        public object GetLastId(Row row, QueryOptions queryOptions)
        {
            IDbCommand command = queryOptions.Connection.CreateCommand();
            command.Transaction = queryOptions.Transaction;

            string sql = $@"
                 select @@IDENTITY Id            
            ";

            command.CommandText = sql;

            return command.ExecuteScalar();
        }

        private Dictionary<string, object> _lastIdMap = new Dictionary<string, object>();

        public string FormatSql(QueryOptions queryOptions)
        {
            string formatedSql = queryOptions.Sql;

            Dictionary<string, object> formats = new Dictionary<string, object>();
            queryOptions.SetFormats(formats);

            foreach (var format in formats) formatedSql = formatedSql.Replace("{{" + format.Key + "}}", format.Value.ToString());

            return formatedSql;
        }

        public void CreateTempTable(QueryOptions queryOptions)
        {

        }

        private IEnumerable<IDbDataParameter> GetParameters(IDbCommand command, QueryOptions queryOptions)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            queryOptions.SetParameters(parameters);

            foreach (var param in parameters)
            {
                IDbDataParameter dbParameter = CreateParameter(command, param.Key, param.Value);
                yield return dbParameter;
            }
        }

        public IDataReader CreateFetchReader(QueryOptions queryOptions)
        {
            CreateTempTable(queryOptions);

            string sql = FormatSql(queryOptions);

            IDbCommand command = queryOptions.Connection.CreateCommand();
            command.Transaction = queryOptions.Transaction;

            command.CommandText = sql;
            foreach (var param in GetParameters(command, queryOptions)) command.Parameters.Add(param);

            return command.ExecuteReader();
        }
    }
}
