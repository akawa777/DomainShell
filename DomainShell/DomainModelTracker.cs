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
            Graph = JsonConvert.SerializeObject(model);
            Tag = tag;
        }

        public object Model { get; private set; }
        public string Graph { get; private set; }
        public object Tag { get; private set; }

        public bool Modified(object model)
        {
            string graph = JsonConvert.SerializeObject(model);

            return Model.GetType() == model.GetType() && Graph == graph;
        }
    }

    public interface IDomainModelMarker
    {
        void Mark(object domainModel);
    }

    public interface IDomainModelTracker : IDomainModelMarker
    {
        TrackPack Get(object domainModel);
        IEnumerable<TrackPack> GetAll();
        void Revoke();
        bool Modified(object domainModel);
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

        public static bool Modified(object domainModel)
        {
            IDomainModelTracker domainModelTracker = _getDomainModelTracker();

            return domainModelTracker.Modified(domainModel);
        }
    }
    

    public abstract class DomainModelTrackerFoundationBase : IDomainModelMarker, IDomainModelTracker
    {
        private OrderedDictionary _list = new OrderedDictionary();
        private object _lock = new object();

        public TrackPack Get(object domainModel)
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

        public IEnumerable<TrackPack> GetAll()
        {
            lock (_lock)
            {
                foreach (DictionaryEntry entry in _list)
                {
                    yield return entry.Value as TrackPack;
                }
            }
        }

        public void Mark(object domainModel)
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

        public void Revoke()
        {
            lock (_lock)
            {
                _list.Clear();
            }
        }

        public bool Modified(object domainModel)
        {
            lock (_lock)
            {
                TrackPack trackPack = Get(domainModel);

                return trackPack.Modified(domainModel);
            }
        }

        protected abstract object CreateTag(object domainModel);
    }
}