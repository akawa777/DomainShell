﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain;

namespace DomainShell.Tests.Infrastructure
{
    public class PersonProxy : PersonEntity, ITransient
    {
        public PersonProxy(PersonDto memento)
        {
            Memento = memento;

            RewriteProxy();
        }

        public PersonDto Memento
        {
            get;
            private set;
        }

        public void RewriteProxy()
        {
            Id = new PersonId(Memento.PersonId);
            Name = Memento.Name;
            EMail = Memento.EMail;
            Address = new AddressValue(Memento.ZipCode, Memento.City);

            if (Memento.HistoryList != null)
            {
                foreach (HistoryDto history in Memento.HistoryList)
                {
                    HistoryEntity historyEntity = new HistoryEntity(Id, history.HistoryNo);
                    historyEntity.Content = history.Content;

                    (HistoryList as List<HistoryEntity>).Add(historyEntity);
                }
            }
        }

        public void RewriteMemento()
        {
            Memento.Name = Name;
            Memento.EMail = EMail;
            Memento.ZipCode = Address.ZipCode;
            Memento.City = Address.City;

            Memento.HistoryList.Clear();

            foreach (HistoryEntity history in HistoryList)
            {
                Memento.HistoryList.Add(new HistoryDto { PersonId = Id.Value, HistoryNo = history.Id.HistoryNo, Content = history.Content });
            }
        }

        public bool Transient { get; set; }
    }
}
