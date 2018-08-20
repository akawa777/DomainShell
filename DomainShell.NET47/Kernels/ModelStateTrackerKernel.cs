using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Specialized;
using DomainShell.Infra;

namespace DomainShell.Kernels
{
    public interface IModelStateTrack
    {
        object DomainModel { get; }
        bool Comiited { get; }
    }

    public interface IModelStateTrackerKernel
    {
        void Mark(object domainModel);
        void Commit(object domainModel);        
        IEnumerable<IModelStateTrack> All();        
        void Clear();
    }

    internal class ModelStateTrack : IModelStateTrack
    {
        public ModelStateTrack(object domainModel)
        {
            DomainModel = domainModel;
        }

        public object DomainModel { get; private set; }

        public bool Comiited { get; private set; }

        public void Commit()
        {
            Comiited = true;
        }
    }

    public class ModelStateTrackerKernel : IModelStateTrackerKernel
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

        public virtual IEnumerable<IModelStateTrack> All()
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

                _list[domainModel] = new ModelStateTrack(domainModel);
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

                var modelStateTrack = _list[domainModel] as ModelStateTrack;

                modelStateTrack.Commit();
            }
        }

        public virtual void Clear()
        {
            lock (_lock)
            {
                _list.Clear();
            }
        }
    }
}