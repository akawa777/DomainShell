﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Commerce.Domain.Contracts
{
    public interface IProductReadDto
    {
        int ProductId { get; set; }
        string ProductName { get; set; }
        decimal Price { get; set; }
    }

    public interface IProductReadService : IInfrastructureService
    {
        IProductReadDto Find(int productId);
    }
}
