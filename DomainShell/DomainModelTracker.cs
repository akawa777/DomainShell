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
    public class TrackPack
    {
        public TrackPack(object model, object tag)
        {
            Model = model;
            Tag = tag;
        }

        public object Model { get; private set; }        
        public object Tag { get; private set; }
    }

    public interface IDomainModelMarker
    {
        void Mark<T>(T domainModel) where T : class;
    }

    public interface IDomainModelTracker : IDomainModelMarker
    {
        TrackPack Get<T>(T domainModel) where T : class;
        IEnumerable<TrackPack> GetAll();
        void Revoke();
    }

    public static class DomainModelMarker
    {
        private static Func<IDomainModelMarker> _getDomainModelMarker;

        public static void Startup(Func<IDomainModelMarker> getDomainModelMarker)
        {            
            _getDomainModelMarker = getDomainModelMarker;
        }

        public static void Mark(object domainModel)
        {
            IDomainModelMarker domainModelMarker = _getDomainModelMarker();
            
            domainModelMarker.Mark(domainModel);
        }
    }

    public static class DomainModelTracker
    {
        private static Func<IDomainModelTracker> _getDomainModelTracker;

        public static void Startup(Func<IDomainModelTracker> getDomainModelTracker)
        {
            _getDomainModelTracker = getDomainModelTracker;
        }

        public static IEnumerable<TrackPack> GetAll()
        {
            IDomainModelTracker domainModelTracker = _getDomainModelTracker();

            return domainModelTracker.GetAll();
        }

        public static void Revoke()
        {
            IDomainModelTracker domainModelTracker = _getDomainModelTracker();

            domainModelTracker.Revoke();
        }
    }
    

    public abstract class DomainModelTrackerFoundationBase : IDomainModelMarker, IDomainModelTracker
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
                    _list.Remove(domainModel);
                }

                _list[domainModel] = new TrackPack(domainModel, CreateTag(domainModel));
            }
        }

        public virtual void Revoke()
        {
            lock (_lock)
            {
                _list.Clear();
            }
        }

        protected abstract object CreateTag<T>(T domainModel) where T : class;
    }
}