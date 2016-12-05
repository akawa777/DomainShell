﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Commerce.Domain.Contracts;

namespace DomainShell.Tests.Commerce.Infrastructure
{
    public class CartItemReadDto : ICartItemReadDto
    {
        public int CustomerId { get; set;}
        public int CartNo { get; set;}
        public int ProductId { get; set;}
        public string ProductName { get; set;}        
        public decimal Price { get; set;}
        public int Quantity { get; set;}
        public decimal TotalPrice { get; set;}
    }
}
