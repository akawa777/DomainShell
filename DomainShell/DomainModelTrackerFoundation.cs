using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Specialized;

namespace DomainShell
{
    public static class DomainModelTracker
    {
        private static Func<IDomainModelTracker> _getDomainModelTracker;

        public static void Startup(Func<IDomainModelTracker> getDomainModelTrackerPerThread)
        {
            _getDomainModelTracker = getDomainModelTrackerPerThread;
        }

        private static void Validate()
        {
            if (_getDomainModelTracker == null)
            {
                throw new InvalidOperationException("StratUp not runninng.");
            }
        }

        public static void Mark(object domainModel)
        {
            Validate();

            IDomainModelTracker domainModelTracker = _getDomainModelTracker();
            
            domainModelTracker.Mark(domainModel);
        }

        public static TrackPack Get<T>(T domainModel) where T : class
        {
            Validate();

            IDomainModelTracker domainModelTracker = _getDomainModelTracker();
            
            return domainModelTracker.Get(domainModel);
        }

        public static IEnumerable<TrackPack> GetAll()
        {
            Validate();

            IDomainModelTracker domainModelTracker = _getDomainModelTracker();

            return domainModelTracker.GetAll();
        }

        public static void Revoke<T>(T domainModel) where T : class
        {
            Validate();

            IDomainModelTracker domainModelTracker = _getDomainModelTracker();

            domainModelTracker.Revoke(domainModel);
        }

        public static void RevokeAll()
        {
            Validate();

            IDomainModelTracker domainModelTracker = _getDomainModelTracker();

            domainModelTracker.RevokeAll();
        }
    }    

    public abstract class DomainModelTrackerFoundationBase : IDomainModelTracker
    {
        private OrderedDictionary _list = new OrderedDictionary();
        private object _lock = new object();

        public virtual TrackPack Get<T>(T domainModel) where T : class
        {
            lock (_lock)
            {
                if (!_list.Contains(domainModel))
                {
                    throw new ArgumentException("domainModel is not marked.");
                }

                return _list[domainModel] as TrackPack;
            }
        }

        public virtual IEnumerable<TrackPack> GetAll()
        {
            lock (_lock)
            {
                foreach (DictionaryEntry entry in _list)
                {
                    yield return entry.Value as TrackPack;
                }
            }
        }

        public virtual void Mark<T>(T domainModel) where T : class
        {
            lock (_lock)
            {
                if (_list.Contains(domainModel))
                {
                    return;
                }

                _list[domainModel] = new TrackPack(domainModel, CreateTag(domainModel));
            }
        }

        public virtual void Revoke<T>(T domainModel) where T : class
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

        protected abstract object CreateTag<T>(T domainModel) where T : class;
    }
}