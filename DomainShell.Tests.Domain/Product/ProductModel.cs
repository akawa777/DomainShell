﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain.Product
{
    public class ProductRecord
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }   

    public class ProductModel : IAggregateRoot
    {
        public ProductModel()
        {

        }

        public ProductModel(ProductRecord record)
        {
            ProductId = record.ProductId;
            ProductName = record.ProductName;
            Price = record.Price;
        }

        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }   
}
