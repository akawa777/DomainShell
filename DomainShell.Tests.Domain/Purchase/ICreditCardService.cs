﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain.Purchase
{
    public interface ICreditCardService
    {
        void Pay(CreditCardValue creditCard, decimal paymentAmount);
    }
}