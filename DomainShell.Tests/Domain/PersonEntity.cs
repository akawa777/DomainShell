using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain.Contracts;

namespace DomainShell.Tests.Domain
{
    public class PersonId : IValue
    {
        protected PersonId()
        {
        }

        public PersonId(string personId)
        {
            Value = personId;
        }

        public string Value { get; protected set; }
    }

    public class PersonEntity : IAggregateRoot<PersonId>
    { 
        public PersonEntity(string id) : this()
        {
            Id = new PersonId(id);            
        }

        protected PersonEntity()
        {            
            HistoryList = new List<HistoryEntity>();
        }
        
        private List<IDomainEvent> _events = new List<IDomainEvent>();

        public virtual PersonId Id
        {
            get;
            protected set;
        }

        public virtual string Name { get; set; }

        public void SetAddressFromZipCode(string zipCode, IZipCodeService service)
        {
            Address = new AddressValue(zipCode, service.GetCityName(zipCode));
        }

        public virtual AddressValue Address { get; set; }

        public virtual string EMail { get; set; }

        public IReadOnlyList<HistoryEntity> HistoryList
        {
            get;
            protected set;
        }

        public HistoryEntity CreateHistory()
        {
            return new HistoryEntity(Id, HistoryList.Count + 1);
        }

        public void AddHisotry(HistoryEntity history)
        {            
            (HistoryList as List<HistoryEntity>).Add(history);            
        }

        public void RemoveHisotry(HistoryEntity history)
        {
            (HistoryList as List<HistoryEntity>).Remove(history);            
        }

        public virtual void Delete()
        {
            _events.Add(new PersonDeletedEvent { PersonId = Id.Value, PersonName = Name, Email = EMail });
        }

        public IEnumerable<IDomainEvent> GetEvents()
        {
            return _events;
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        public virtual void Validate(IValidationSpec<PersonEntity, string> spec)
        {
            string[] errors;
            if (!spec.Validate(this, out errors))
            {
                throw new Exception(errors.First());
            }
        }
    }
}
