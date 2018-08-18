using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Specialized;

namespace DomainShell.Kernels
{
    public interface IModelStateTrackerKernel
    {
        void Mark(object domainModel);
        void Commit(object domainModel);
        IModelStateTrack Get(object domainModel);
        IEnumerable<IModelStateTrack> GetAll();
        void Revoke(object domainModel);
        void RevokeAll();
    }

    internal class ModelStateTrack : IModelStateTrack
    {
        public ModelStateTrack(object domainModel, object tag)
        {
            DomainModel = domainModel;
            Tag = tag;
        }

        public object DomainModel { get; private set; }
        public object Tag { get; private set; }

        public bool Comiited { get; private set; }

        public void Commit()
        {
            Comiited = true;
        }
    }

    public abstract class ModelStateTrackerKernelBase : IModelStateTrackerKernel
    {
        private OrderedDictionary _list = new OrderedDictionary();
        private object _lock = new object();

        public virtual IModelStateTrack Get(object domainModel)
        {
            lock (_lock)
            {
                if (!_list.Contains(domainModel))
                {
                    throw new ArgumentException("domainModel is not marked.");
                }

                return _list[domainModel] as IModelStateTrack;
            }
        }

        public virtual IEnumerable<IModelStateTrack> GetAll()
        {
            lock (_lock)
            {
                foreach (DictionaryEntry entry in _list)
                {
                    yield return entry.Value as IModelStateTrack;
                }
            }
        }

        public virtual void Mark(object domainModel)
        {
            lock (_lock)
            {
                if (_list.Contains(domainModel))
                {
                    return;
                }

                _list[domainModel] = new ModelStateTrack(domainModel, CreateTag(domainModel));
            }
        }

        public void Commit(object domainModel)
        {
            lock (_lock)
            {
                if (!_list.Contains(domainModel))
                {
                    throw new ArgumentException("domainModel is not marked.");
                }

                var modelStateTrack = _list[domainModel] as IModelStateTrack;

                modelStateTrack.Commit();
            }
        }

        public virtual void Revoke(object domainModel)
        {
            lock (_lock)
            {
                if (!_list.Contains(domainModel))
                {
                    throw new ArgumentException("domainModel is not marked.");
                }

                _list.Remove(domainModel);
            }
        }

        public virtual void RevokeAll()
        {
            lock (_lock)
            {
                _list.Clear();
            }
        }

        protected abstract object CreateTag(object domainModel);
    }
}