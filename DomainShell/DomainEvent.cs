﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell
{
    public interface IDomainEvent
    {
        
    }

    public interface IDomainOuterTranEvent : IDomainEvent
    {
        bool Async { get; set; }
    }

    public interface IDomainExceptionEvent : IDomainEvent
    {
        Exception Exception { get; set; }
    }

    public interface IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        void Handle(TDomainEvent domainEvent);
    }


    public interface IDomainEventScope : IDisposable
    {
        IDomainEventHandler<TDomainEvent> GetHandler<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;        
    }

    public interface IDomainEventAuthor
    {
        IEnumerable<IDomainEvent> GetEvents();
        void ClearEvents();
    }
}
