using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell
{
    public interface ISession
    {
        IOpenScope Open();
        ITranScope Tran();
        void OnException(Exception exception);
    }

    public interface IOpenScope : IDisposable
    {
        
    }

    public interface ITranScope : IDisposable
    {
        void Complete();
    }

    public interface IConnection
    {
        void Open();
        void Close();
        void BeginTran();
        void BeginCommit();
        void Commit();
        void Rollback();
        void DisposeTran(bool completed);
        void Dispose();
    }
}