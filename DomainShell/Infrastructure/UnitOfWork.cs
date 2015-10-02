using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Event;
using DomainShell.EventDispatch;
using DomainShell.Domain;

namespace DomainShell.Infrastructure
{
    public interface ITransactionProcessor
    {
        void Execute(Action saveAction);
    }

    internal class TransactionProcessor : ITransactionProcessor
    {
        public void Execute(Action saveAction)
        {
            saveAction();
        }
    }

    public interface IUnitOfWork
    {
        void Save(params IAggregateRoot[] aggregateRoots);
    }

    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IDomainEventPublisher domainEventPublisher) : this(domainEventPublisher, new TransactionProcessor())
        {

        }
        public UnitOfWork(IDomainEventPublisher domainEventPublisher, ITransactionProcessor transactionProcessor)
        {
            _domainEventPublisher = domainEventPublisher;
            _transactionProcessor = transactionProcessor;
        }

        private IDomainEventPublisher _domainEventPublisher;
        private ITransactionProcessor _transactionProcessor;

        public void Save(params IAggregateRoot[] aggregateRoots)
        {
            List<IDomainEvent> events = new List<IDomainEvent>();
            List<IDomainEvent> afterTransactionEvents = new List<IDomainEvent>();

            foreach (IAggregateRoot aggregateRoot in aggregateRoots)
            {
                EventList eventList = aggregateRoot.EventList;
                events.AddRange(eventList);

                foreach (var keyValue in (eventList as IDomainEventCallbackCache).GetCallbackCache())
                {
                    _domainEventPublisher.Callback(keyValue.Key as dynamic, keyValue.Value);
                }

                aggregateRoot.EventList.Clear();
            }

            Action action = () =>
            {
                foreach (IDomainEvent @event in events)
                {
                    if (@event.InTransaction())
                    {
                        _domainEventPublisher.Publish(@event as dynamic);
                    }
                    else
                    {
                        afterTransactionEvents.Add(@event);
                    }
                }
            };

            _transactionProcessor.Execute(action);

            foreach (IDomainEvent @event in afterTransactionEvents)
            {
                _domainEventPublisher.Publish(@event as dynamic);
            }
        }
    }
}


