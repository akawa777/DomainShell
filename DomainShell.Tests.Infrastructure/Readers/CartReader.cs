using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DomainShell.Tests.Domain.ReadModels;

namespace DomainShell.Tests.Infrastructure.Readers
{
    public class CartReader
    {
        public CartReader(Session session)
        {
            _session = session;
        }

        private Session _session;

        public List<CartReadModel> GetAll()
        {
            return new List<CartReadModel>();
        }

        public List<CartDetailReadModel> GetDetailList(string cartId)
        {
            return new List<CartDetailReadModel>();
        }
    }
}
