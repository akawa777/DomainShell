using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell
{
    public struct DomainEventMode
    {
        private DomainEventMode(DomainEventFormat format)
        {
            _format = format;
            _exception = null;
        }

        private DomainEventFormat _format;
        public DomainEventFormat Format
        {
            get { return _format; }
            set { _format = value;  }
        }

        private Exception _exception;
        public Exception GetException()
        {
            return _exception;
        }

        public static DomainEventMode InTran()
        {
            return new DomainEventMode(DomainEventFormat.InTran);
        }

        public static DomainEventMode OutTran()
        {
            return new DomainEventMode(DomainEventFormat.OutTran);
        }

        public static DomainEventMode ByAsync()
        {
            return new DomainEventMode(DomainEventFormat.ByAsync);
        }

        public static DomainEventMode AtException()
        {
            return new DomainEventMode(DomainEventFormat.AtException);
        }
    }

    public enum DomainEventFormat
    {
        InTran,
        OutTran,
        ByAsync,
        AtException         
    }

    public interface IDomainEvent
    {
        DomainEventMode Mode { get; }
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

    public interface IDomainEventPublisher
    {        
        void PublishInTran();
        void PublishOutTran();
        void PublishOnException(Exception exception);
        void Revoke();
    }
}
