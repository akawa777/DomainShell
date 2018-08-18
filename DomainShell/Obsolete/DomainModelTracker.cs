//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using System.Collections;
//using System.Collections.Specialized;
//using DomainShell.Kernels;

//namespace DomainShell
//{
//    public static class DomainModelTracker
//    {
//        private static Func<IDomainModelTrackerKernel> _getDomainModelTrackerKernel;

//        public static void Startup(Func<IDomainModelTrackerKernel> getDomainModelTrackerKernel)
//        {
//            _getDomainModelTrackerKernel = getDomainModelTrackerKernel;
//        }

//        private static void Validate()
//        {
//            if (_getDomainModelTrackerKernel == null)
//            {
//                throw new InvalidOperationException("StratUp not runninng.");
//            }
//        }

//        public static void Mark(object domainModel)
//        {
//            Validate();

//            var domainModelTracker = _getDomainModelTrackerKernel();

//            domainModelTracker.Mark(domainModel);
//        }

//        public static TrackPack Get(object domainModel)
//        {
//            Validate();

//            var domainModelTracker = _getDomainModelTrackerKernel();

//            return domainModelTracker.Get(domainModel);
//        }

//        public static IEnumerable<TrackPack> GetAll()
//        {
//            Validate();

//            var domainModelTracker = _getDomainModelTrackerKernel();

//            return domainModelTracker.GetAll();
//        }

//        public static void Revoke(object domainModel)
//        {
//            Validate();

//            var domainModelTracker = _getDomainModelTrackerKernel();

//            domainModelTracker.Revoke(domainModel);
//        }

//        public static void RevokeAll()
//        {
//            Validate();

//            var domainModelTracker = _getDomainModelTrackerKernel();

//            domainModelTracker.RevokeAll();
//        }
//    }
//}