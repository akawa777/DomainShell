using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell
{
    public interface IDomainModelMarker
    {
        void Mark<T>(T domainModel) where T : class;
    }

    public interface IDomainModelTracker : IDomainModelMarker
    {
        IEnumerable<T> Get<T>() where T : class;
        IEnumerable<object> GetAll();
        void Revoke();
    }

    public static class DomainModelMarker
    {
        private static Func<IDomainModelMarker> _getDomainModelMarker;

        public static void Startup(Func<IDomainModelMarker> getDomainModelMarker)
        {            
            _getDomainModelMarker = getDomainModelMarker;
        }

        public static void Mark<T>(T domainModel) where T : class
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

        public static IEnumerable<T> Get<T>() where T : class
        {
            IDomainModelTracker domainModelTracker = _getDomainModelTracker();

            return domainModelTracker.Get<T>();
        }

        public static IEnumerable<object> GetAll()
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

    public class DomainModelTrackerFoundation : IDomainModelMarker, IDomainModelTracker
    {
        Dictionary<object, object> _list = new Dictionary<object, object>();

        public IEnumerable<T> Get<T>() where T : class
        {
            return _list.Where(x => x.Value is T).Select(x => x.Value as T);
        }

        public IEnumerable<object> GetAll()
        {
            return _list.Values;
        }

        public void Mark<T>(T domainModel) where T : class
        {
            _list[domainModel] = domainModel;
        }

        public void Revoke()
        {
            _list.Clear();
        }
    }

}