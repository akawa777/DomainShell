using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DomainShell
{
    public class TrackPack
    {
        public TrackPack(object model, object stamp)
        {
            Model = model;
            Graph = JsonConvert.SerializeObject(model);
            Stamp = stamp;
        }

        public object Model { get; private set; }
        public string Graph { get; private set; }
        public object Stamp { get; private set; }

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
        private Dictionary<object, TrackPack> _list = new Dictionary<object, TrackPack>();
        private object _lock = new object();

        public TrackPack Get(object domainModel)
        {
            TrackPack trackPack;

            if (!_list.TryGetValue(domainModel, out trackPack))
            {
                return null;
            }

            return trackPack;
        }

        public IEnumerable<TrackPack> GetAll()
        {
            return _list.Values;
        }

        public void Mark(object domainModel)
        {
            lock (_lock)
            {
                object stamp = CreateStamp(domainModel);
                _list[domainModel] = new TrackPack(domainModel, stamp);
            }
        }

        public void Revoke()
        {
            _list.Clear();
        }

        public bool Modified(object domainModel)
        {
            TrackPack trackPack;

            if (!_list.TryGetValue(domainModel, out trackPack))
            {
                return false;
            }

            return trackPack.Modified(domainModel);
        }

        protected abstract object CreateStamp(object domainModel);
    }
}