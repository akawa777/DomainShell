using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain
{
    public class HistoryId : IValue
    {
        protected HistoryId()
        {

        }

        public HistoryId(PersonId personId, int historyNo)
        {
            PersonId = personId;
            HistoryNo = historyNo;
        }

        public PersonId PersonId { get; protected set; }
        public int HistoryNo { get; protected set; }

        public string Value
        {
            get { return string.Join(":", PersonId, HistoryNo); }
        }
    }

    public class HistoryEntity : IEntity<HistoryId>
    {
        protected HistoryEntity()
        {

        }

        public HistoryEntity(PersonId personId, int historyNo)
        {            
            HistoryId id = new HistoryId(personId, historyNo);
            Id = id;
        }

        public virtual HistoryId Id
        {
            get;
            protected set;
        }

        public virtual string Content
        {
            get;
            set;
        }
    }
}
