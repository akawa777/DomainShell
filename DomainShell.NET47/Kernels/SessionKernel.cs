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

    public abstract class SessionKernelBase : ISessionKernel
    {   
        private IOpenScope _openScope = null;
        private ITranScope _tranScope = null;
        private object _lockOpen = new object();
        private object _lockTran = new object();  

        private void ValidateComiited()
        {
            try
            {
                foreach (IModelStateTrack modelStateTrack in ModelStateTracker.Current.All())
                {
                    if (!modelStateTrack.IsComiited && !modelStateTrack.IsRollbacked)
                    {
                        throw new InvalidOperationException($"{modelStateTrack.DomainModel.GetType()} is sealed. but not commited.");
                    }
                }
            }
            catch
            {
                ModelStateTracker.Current.Clear();
                throw;
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
                            catch
                            {
                                EndOpen();
                                throw;
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
                            catch
                            {
                                EndTran(false);
                                throw;
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
            DomainEventPublisher.Current.PublishOnException(exception);
        }

        protected abstract void BeginOpen();
        protected abstract void BeginTran();
        protected abstract void EndTran(bool completed);
        protected abstract void EndOpen();        
    }
}