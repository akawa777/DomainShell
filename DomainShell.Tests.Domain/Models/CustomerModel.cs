﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain.Models
{
    public class CustomerModel : IAggregateRoot
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }        

        public State State { get; private set; }

        public void Create()
        {
            State = State.Created;
        }

        public void Update()
        {
            State = State.Updated;
        }

        public void  Delete()
        {
            State = State.Deleted;            
        }

        public void Accepted()
        {
            State = State.UnChanged;
        }
    }
}
