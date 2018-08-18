//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using System.Collections;
//using System.Collections.Specialized;

//namespace DomainShell.Kernels
//{
//    public class TrackPack
//    {
//        public TrackPack(object domainModel, object tag)
//        {
//            DomainModel = domainModel;
//            Tag = tag;
//        }

//        public object DomainModel { get; private set; }        
//        public object Tag { get; private set; }
//    }

//    public interface IDomainModelTrackerKernel
//    {
//        void Mark(object domainModel);
//        TrackPack Get(object domainModel);
//        IEnumerable<TrackPack> GetAll();
//        void Revoke(object domainModel);
//        void RevokeAll();
//    }

//    public abstract class DomainModelTrackerKernelBase : IDomainModelTrackerKernel
//    {
//        private OrderedDictionary _list = new OrderedDictionary();
//        private object _lock = new object();

//        public virtual TrackPack Get(object domainModel)
//        {
//            lock (_lock)
//            {
//                if (!_list.Contains(domainModel))
//                {
//                    throw new ArgumentException("domainModel is not marked.");
//                }

//                return _list[domainModel] as TrackPack;
//            }
//        }

//        public virtual IEnumerable<TrackPack> GetAll()
//        {
//            lock (_lock)
//            {
//                foreach (DictionaryEntry entry in _list)
//                {
//                    yield return entry.Value as TrackPack;
//                }
//            }
//        }

//        public virtual void Mark(object domainModel)
//        {
//            lock (_lock)
//            {
//                if (_list.Contains(domainModel))
//                {
//                    return;
//                }

//                _list[domainModel] = new TrackPack(domainModel, CreateTag(domainModel));
//            }
//        }

//        public virtual void Revoke(object domainModel)
//        {
//            lock (_lock)
//            {
//                if (!_list.Contains(domainModel))
//                {
//                    throw new ArgumentException("domainModel is not marked.");
//                }

//                _list.Remove(domainModel);
//            }
//        }

//        public virtual void RevokeAll()
//        {
//            lock (_lock)
//            {
//                _list.Clear();
//            }
//        }

//        protected abstract object CreateTag(object domainModel);
//    }
//}