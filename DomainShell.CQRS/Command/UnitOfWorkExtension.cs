using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Infrastructure;

namespace DomainShell.CQRS.Command
{
    public static class UnitOfWorkExtension
    {
        public static Task AsyncSave(this UnitOfWork unitOfWork, params IAggregateRoot[] aggregateRoots) 
        {
            Task task = Task.Run(() => unitOfWork.Save(aggregateRoots));

            return task;
        }
    }
}