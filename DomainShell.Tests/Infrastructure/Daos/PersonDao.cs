using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dagent;

namespace DomainShell.Tests.Infrastructure.Daos
{
    public class PersonDao
    {
        public PersonDao(System.Data.Common.DbConnection connection)
        {
            _connection = connection;
        }

        private System.Data.Common.DbConnection _connection;

        private string _sql = @"
                select
                    Person.PersonId,
                    Person.Name,
                    Person.EMail,
                    Person.ZipCode,
                    Person.City,
                    History.HistoryNo,
                    History.Content
                from
                    Person
                left join
                    History
                on
                    Person.PersonId = History.PersonId
                {0}
                order by
                    Person.PersonId,
                    History.HistoryNo
        ";

        public PersonDto Find(string personId)
        {
            DagentDatabase db = new DagentDatabase(_connection);

            string sql = string.Format(_sql, "where Person.PersonId = @personId");                

            PersonDto personDto = db.Query<PersonDto>(sql, new { personId = personId })
                .Unique("PersonId")                
                .Each((dto, rows) => rows.Map(dto, x => x.HistoryList, "PersonId", "HistoryNo").Do())
                .Single();

            return personDto;
        }

        public enum PredicateItem
        {
            LikeName,
            City
        }

        public class Predicate : Dictionary<PredicateItem, object>
        {
            public bool And { get; set; }
        }

        public IEnumerable<PersonDto> GetList(Predicate predicate)
        {
            DagentDatabase db = new DagentDatabase(_connection);

            string sql = _sql;

            StringBuilder where = new StringBuilder();

            int suffix = 1;

            List<Parameter> parameters = new List<Parameter>();

            foreach (KeyValuePair<PredicateItem, object> keyValue in predicate)
            {
                if (where.ToString() != string.Empty)
                {
                    where.Append(predicate.And ? " and " : " or ");
                    where.Append(Environment.NewLine);
                }

                string paramName = string.Empty;
                if (keyValue.Key == PredicateItem.LikeName)
                {
                    paramName = string.Format("name_{0}", suffix);
                    where.Append(string.Format("Person.Name like @{0}", paramName));

                    parameters.Add(new Parameter(paramName, keyValue.Value.ToString() + "%"));
                }
                else if (keyValue.Key == PredicateItem.City)
                {
                    paramName = string.Format("city_{0}", suffix);
                    where.Append(string.Format("Person.City = @{0}", paramName));

                    parameters.Add(new Parameter(paramName, keyValue.Value));
                }
            }

            if (where.ToString() != string.Empty)
            {
                sql = string.Format(sql, "where " + where.ToString());
            }

            IEnumerable<PersonDto> personDtos = db.Query<PersonDto>(sql, parameters.ToArray())
                .Unique("PersonId")
                .Each((dto, rows) => rows.Map(dto, x => x.HistoryList, "PersonId", "HistoryNo").Do())
                .EnumerateList();

            return personDtos;
        }

        public void Insert(PersonDto personDto)
        {
            DagentDatabase db = new DagentDatabase(_connection);

            db.Command<PersonDto>("Person", "PersonId").Insert(personDto);

            foreach (HistoryDto historyDto in personDto.HistoryList)
            {
                db.Command<HistoryDto>("History", "PersonId", "HistoryNo").Insert(historyDto);
            }
        }

        public void Update(PersonDto personDto)
        {
            DagentDatabase db = new DagentDatabase(_connection);

            db.Command<PersonDto>("Person", "PersonId").Update(personDto);

            db.ExequteNonQuery("delete from History where PersonId = @personId", new Parameter("personId", personDto.PersonId));

            foreach (HistoryDto historyDto in personDto.HistoryList)
            {
                db.Command<HistoryDto>("History", "PersonId", "HistoryNo").Insert(historyDto);
            }
        }

        public void Delete(PersonDto personDto)
        {
            DagentDatabase db = new DagentDatabase(_connection);

            db.Command<PersonDto>("Person", "PersonId").Delete(personDto);

            db.ExequteNonQuery("delete from History where PersonId = @personId", new Parameter("personId", personDto.PersonId));
        }

        public IEnumerable<PersonViewDto> GetViewList()
        {
            DagentDatabase db = new DagentDatabase(_connection);

            string sql = string.Format(_sql, string.Empty);

            IEnumerable<PersonViewDto> personDtos = db.Query<PersonViewDto>(sql).EnumerateList();

            return personDtos;
        }
    }
}
