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

    public interface IDomainModelTracker
    {
        void Mark<T>(T domainModel) where T : class;
        TrackPack Get<T>(T domainModel) where T : class;
        IEnumerable<TrackPack> GetAll();
        void Revoke<T>(T domainModel) where T : class;
        void RevokeAll();
    }
}