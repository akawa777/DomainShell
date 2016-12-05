using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.Domain.Contracts
{
    public interface IPersonReadDto
    {
        string PersonId { get; set; }
        string Name { get; set; }
        string EMail { get; set; }
        string ZipCode { get; set; }
        string City { get; set; }
        int HistoryNo { get; set; }
        string Content { get; set; }
    }

    public interface IPersonReadService
    {
        IEnumerable<IPersonReadDto> GetPersonList();
    }
}
