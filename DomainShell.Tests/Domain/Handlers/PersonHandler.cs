using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain;
using DomainShell.Tests.Domain.Contracts;

namespace DomainShell.Tests.Domain.Handlers
{
    public class PersonEventHandler : IDomianEventHandler<PersonDeletedEvent>
    {
        public PersonEventHandler(IMailService mailService)
        {            
            _mailService = mailService;
        }
        
        private IMailService _mailService;

        public void Handle(PersonDeletedEvent domainEvent)
        {
            _mailService.Send(domainEvent.Email, "xxx", "xxxxxxxxxxxxxxxxx");
        }
    }
}
