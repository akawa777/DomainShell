using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain.Contracts
{
    public interface IMailService : IInfrastructureService
    {
        void Send(string address, string title, string content);
    }
}
