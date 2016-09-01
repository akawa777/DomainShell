using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Infrastructure
{
    public interface ISessionKernel
    {
        Type GetConnectionPortType();
        object GetConnectionPort();
        void Open();
        void Close();
        void BeginTran();
        void Commit();
        void Rollback();
    }

    public class Session
    {
        public Session(params ISessionKernel[] kernels)
        {
            foreach (ISessionKernel kernel in kernels)
            {
                _kernelMap[kernel.GetConnectionPortType()] = kernel;
            }
        }

        private Dictionary<Type, ISessionKernel> _kernelMap = new Dictionary<Type, ISessionKernel>();

        public TConnection GetConnectionPort<TConnection>() where TConnection : class
        {
            if (_kernelMap.ContainsKey(typeof(TConnection)))
            {
                return _kernelMap[typeof(TConnection)].GetConnectionPort() as TConnection;
            }

            return null;
        }

        public IConnection Connect()
        {
            foreach (ISessionKernel kernel in _kernelMap.Values)
            {
                kernel.Open();
            }

            return new Connection(_kernelMap);
        }

        public ITran Tran()
        {
            foreach (ISessionKernel kernel in _kernelMap.Values)
            {
                kernel.Open();
                kernel.BeginTran();
            }

            return new Tran(_kernelMap);
        }
    }

    public interface IConnection : IDisposable
    {

    }

    internal class Connection : IConnection
    {
        public Connection(Dictionary<Type, ISessionKernel> kernelMap)
        {
            _kernelMap = kernelMap;
        }

        private Dictionary<Type, ISessionKernel> _kernelMap = new Dictionary<Type, ISessionKernel>();

        public void Dispose()
        {
            foreach (ISessionKernel kernel in _kernelMap.Values)
            {
                kernel.Close();
            }
        }
    }

    public interface ITran : IDisposable
    {
        void Complete();
    }

    internal class Tran : ITran
    {
        public Tran(Dictionary<Type, ISessionKernel> kernelMap)
        {
            _kernelMap = kernelMap;
        }

        private Dictionary<Type, ISessionKernel> _kernelMap = new Dictionary<Type, ISessionKernel>();
        private bool completed = false;

        public void Complete()
        {
            foreach (ISessionKernel kernel in _kernelMap.Values)
            {
                kernel.Commit();
            }

            completed = true;
        }

        public void Dispose()
        {
            if (!completed)
            {
                foreach (ISessionKernel kernel in _kernelMap.Values)
                {
                    kernel.Rollback();
                }
            }

            foreach (ISessionKernel kernel in _kernelMap.Values)
            {
                kernel.Close();
            }
        }
    }
}
