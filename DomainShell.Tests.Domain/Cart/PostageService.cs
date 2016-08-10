using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.Domain.Cart
{
    public interface IPostageService
    {
        decimal GetPostage(string shippingAddress);
    }

    public class PostageService : IPostageService
    {
        public decimal GetPostage(string shippingAddress)
        {
            return 0;
        }
    }
}
