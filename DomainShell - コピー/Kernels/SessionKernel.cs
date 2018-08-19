using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Infra;
using DomainShell.App;

namespace DomainShell.Kernels
{
    public interface ISessionKernel
    {
        IOpenScope Open();
        ITranScope Tran();
        void OnException(Exception exception);
    }

    internal class OpenScope : IOpenScope
    {
        public OpenScope(Action dispose)
        {
            _dispose = dispose;
        }

        private Action _dispose = () => { };

        public void Dispose()
        {
            _dispose();
        }
    }

    internal class TranScope : ITranScope
    {
        public TranScope(Action<bool> dispose)
        {
            _dispose = dispose;
        }

        private Action<bool> _dispose = x => { };
        private bool _completed = false;

        public void Complete()
        {
            _completed = true;
        }

        public void Dispose()
        {
            try
            {
                _dispose(_completed);
            }
            finally
            {
                _completed = false;
            }
        }
    }

    public abstract class SessionKernelBase<TDomainEvent> : ISessionKernel
    {
        public SessionKernelBase(IDomainEventCacheKernel<TDomainEvent> domainEventCacheKernel)
        {
            _modelStateTrackerKernel = ModelStateTracker.Current;
            _domainEventCacheKernel = domainEventCacheKernel;
        }

        private readonly IModelStateTrackerKernel _modelStateTrackerKernel;
        private readonly IDomainEventCacheKernel<TDomainEvent> _domainEventCacheKernel;
        private IOpenScope _openScope = null;
        private ITranScope _tranScope = null;
        private object _lockOpen = new object();
        private object _lockTran = new object();  

        private void ValidateComiited()
        {
            foreach (IModelStateTrack modelStateTrack in _modelStateTrackerKernel.GetAll())
            {
                if (!modelStateTrack.Comiited)
                {
                    throw new InvalidOperationException($"{modelStateTrack.DomainModel.GetType()} is sealed. but not commited.");
                }
            }
        }

        public virtual IOpenScope Open()
        {
            lock (_lockOpen)
            {
                if (_openScope == null)
                {
                    BeginOpen();

                    _openScope = new OpenScope(() =>
                    {
                        lock (_lockOpen)
                        {
                            try
                            {
                                ValidateComiited();

                                EndOpen();                                
                            }
                            catch (Exception e)
                            {
                                EndOpen();
                                throw e;
                            }
                            finally
                            {
                                _openScope = null;
                            }
                        }
                    });

                    return _openScope;
                }

                return new OpenScope(() => { });
            }
        }

        public virtual ITranScope Tran()
        {
            lock (_lockTran)
            {
                var openScope = Open();

                if (_tranScope == null)
                {
                    BeginTran();

                    _tranScope = new TranScope(
                    completed =>
                    {
                        lock (_lockTran)
                        {
                            try
                            {   
                                ValidateComiited();

                                EndTran(completed);
                            }
                            catch(Exception e)
                            {
                                EndTran(false);
                                throw e;
                            }
                            finally
                            {
                                _tranScope = null;
                                openScope.Dispose();
                            }
                        }
                    });

                    return _tranScope;
                }

                return new TranScope(
                completed => { });
            }
        }

        public void OnException(Exception exception)
        {
            try
            {
                HandleDomainEventsOnException(exception, _domainEventCacheKernel.ToArray());
                OnException(exception);
            }
            catch (Exception e)
            {
                var aggregateException = new AggregateException(e, exception);
                throw aggregateException;
            }
            finally
            {
                _domainEventCacheKernel.Clear();
            }
        }

        protected abstract void BeginOpen();
        protected abstract void BeginTran();
        protected abstract void EndTran(bool completed);
        protected abstract void EndOpen();
        protected abstract void HandleDomainEventsOnException(Exception exception, TDomainEvent[] domainEvents);
    }
}