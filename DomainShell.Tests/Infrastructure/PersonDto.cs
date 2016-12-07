using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.Infrastructure
{
    public class PersonDto
    {
        public PersonDto()
        {
            HistoryList = new List<HistoryDto>();
        }

        public string PersonId { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }

        public List<HistoryDto> HistoryList { get; set; }
    }

    public class HistoryDto
    {
        public string PersonId { get; set; }
        public int HistoryNo { get; set; }
        public string Content { get; set; }
    }

    public class PersonReadDto
    {
        public string PersonId { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public int HistoryNo { get; set; }
        public string Content { get; set; }
    }
}
