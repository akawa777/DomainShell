using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Infrastructure
{
    public class Session : ISession
    {
        public Session(params ISessionKernel[] kernels)
        {
            foreach (ISessionKernel kernel in kernels)
            {
                _kernelMap[kernel.GetType().GetInterface(typeof(ISessionKernel<>).FullName).GetGenericArguments()[0]] = kernel;
            }
        }

        private Dictionary<Type, ISessionKernel> _kernelMap = new Dictionary<Type, ISessionKernel>();        

        public TConnection GetPort<TConnection>() where TConnection : class
        {
            if (_kernelMap.ContainsKey(typeof(TConnection)))
            {
                return (_kernelMap[typeof(TConnection)] as dynamic).GetConnectionPort() as TConnection;
            }

            return null;
        }

        public IConnection Open()
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

            return new Tran(_kernelMap, false);
        }
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

        public ITran Tran()
        {
            foreach (ISessionKernel kernel in _kernelMap.Values)
            {   
                kernel.BeginTran();
            }

            return new Tran(_kernelMap, true);
        }
    }

    internal class Tran : ITran
    {
        public Tran(Dictionary<Type, ISessionKernel> kernelMap, bool opend)
        {
            _kernelMap = kernelMap;
            _opend = opend;
        }

        private Dictionary<Type, ISessionKernel> _kernelMap = new Dictionary<Type, ISessionKernel>();
        private bool _completed = false;
        private bool _opend = false;

        public void Complete()
        {
            foreach (ISessionKernel kernel in _kernelMap.Values)
            {
                kernel.Commit();
            }

            _completed = true;
        }

        public void Dispose()
        {
            if (!_completed)
            {
                foreach (ISessionKernel kernel in _kernelMap.Values)
                {
                    kernel.Rollback();
                }
            }

            if (!_opend)
            {
                foreach (ISessionKernel kernel in _kernelMap.Values)
                {
                    kernel.Close();
                }
            }
        }
    }
}
