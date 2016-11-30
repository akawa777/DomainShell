using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain;
using DomainShell.Tests.Domain.Contracts;
using DomainShell.Tests.Infrastructure;
using DomainShell.Tests.Infrastructure.Contracts;

namespace DomainShell.Tests.App
{
    public class PersonCreationRequest
    {
        public string PersonId { get; set; }
        public string Name { get; set; }
        public string ZipCode { get; set; }
        public string EMail { get; set; }
        public string Content { get; set; }
    }

    public class PersonUpdateRequest
    {        
        public string Name { get; set; }
        public string City { get; set; }
        public string Content { get; set; }
    }

    public class PersonDeletionRequest
    {
        public string PersonId { get; set; }
    }

    public class PersonApp
    {
        public PersonApp()
        {
            _session = new Session();

            DomainEventDispatcher domainEventDispatcher = new DomainEventDispatcher();

            _factory = new Infrastructure.Factories.PersonFactory();
            _repository = new Infrastructure.Repositories.PersonRepository(_session, domainEventDispatcher);
            _zipCodeService = new Infrastructure.Services.ZipCodeService();
            _reader = new Infrastructure.Services.PersonViewReader(_session);

            domainEventDispatcher.Register<PersonDeletedEvent>(new Domain.Handlers.PersonEventHandler(new Infrastructure.Services.MailService()));            
        }

        private ISession _session;

        private IPersonFactory _factory;        
        private IPersonRepository _repository;
        private IZipCodeService _zipCodeService;
        private IPersonViewReader _reader;

        public void Create(PersonCreationRequest request)
        {
            using (ITran tran = _session.Tran())
            {
                PersondCreationSpec spec = new PersondCreationSpec(request.PersonId);
                PersonEntity person = _factory.Create(spec);

                person.Name = request.Name;

                person.SetAddressFromZipCode(request.ZipCode, _zipCodeService);

                HistoryEntity history = person.CreateHistory();
                history.Content = request.Content;

                person.AddHisotry(history);

                PersonValidationSpec validationSpec = new PersonValidationSpec();
                person.Validate(validationSpec);
                
                _repository.Save(person);

                tran.Complete();
            }
        }

        public void Update(PersonUpdateRequest request)
        {
            using (ITran tran = _session.Tran())
            {
                PersonLikeNameSelectionSpec spec = new PersonLikeNameSelectionSpec(request.Name);
                PersonEntity person = _repository.List(spec).FirstOrDefault();
                
                person.Address = new AddressValue(person.Address.ZipCode, request.City);

                person.HistoryList[0].Content = request.Content;

                PersonValidationSpec validationSpec = new PersonValidationSpec();
                person.Validate(validationSpec);

                _repository.Save(person);

                tran.Complete();
            }
        }

        public void Delete(PersonDeletionRequest request)
        {
            using (ITran tran = _session.Tran())
            {
                PersonEntity person = _repository.Find(new PersonId(request.PersonId));

                person.Delete();

                _repository.Save(person);

                tran.Complete();
            }
        }

        public IEnumerable<PersonViewDto> GetCollection()
        {
            using (_session.Open())
            {
                return _reader.GetPersonViewList();               
            }
        }
    }
}
