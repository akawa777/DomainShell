using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Infrastructure
{
    public interface ISessionKernel
    {   
        void Open();
        void Close();
        void BeginTran();
        void Commit();
        void Rollback();
    }

    public interface ISessionKernel<TConnectionPort> : ISessionKernel
    {        
        TConnectionPort GetConnectionPort();
    }

    public interface ISession
    {
        TConnection GetPort<TConnection>() where TConnection : class;
        IConnection Open();
        ITran Tran();
    }

    public interface IConnection : IDisposable
    {
        ITran Tran();
    }

    public interface ITran : IDisposable
    {
        void Complete();
    }
}
