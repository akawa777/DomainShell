using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dagent;
using DomainShell.Domain;
using DomainShell.Tests.Domain;

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

        public IEnumerable<PersonDto> GetList<TTarget>(PredicateNode<TTarget, Operator> predicate)
        {
            SqlGenerator sqlGenerator = new SqlGenerator();
            var where = sqlGenerator.Generate(predicate);

            DagentDatabase db = new DagentDatabase(_connection);

            string sql = _sql;

            if (where != string.Empty)
            {
                sql = string.Format(sql, "where " + where.ToString());
            }

            List<Parameter> dbParameters = new List<Parameter>();

            foreach (PredicateNode.Parameter parameter in predicate.Parameters)
            {
                dbParameters.Add(new Parameter(predicate.ParameterName, predicate.Value));
            }

            IEnumerable<PersonDto> personDtos = db.Query<PersonDto>(sql, dbParameters.ToArray())
                .Unique("PersonId")
                .Each((dto, rows) => 
                {
                    rows.Map(dto, x => x.HistoryList, "PersonId", "HistoryNo").Do();
                })
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

        public IEnumerable<PersonReadDto> GetViewList()
        {
            DagentDatabase db = new DagentDatabase(_connection);

            string sql = string.Format(_sql, string.Empty);

            IEnumerable<PersonReadDto> personDtos = db.Query<PersonReadDto>(sql).EnumerateList();

            return personDtos;
        }
    }
}
