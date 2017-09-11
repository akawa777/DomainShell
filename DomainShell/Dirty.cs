using System;
using Newtonsoft.Json;

namespace DomainShell
{    
    public sealed class Dirty
    {
        private Dirty()
        {   
        }

        private Dirty(object domainModel)
        {
            _domainModel = domainModel;
            _serializedData = SerializeData();
            
            DomainModelTracker.Mark(domainModel);
        }

        private object _domainModel;
        private string _serializedData;
        
        private string SerializeData()
        {
            return JsonConvert.SerializeObject(_domainModel, Formatting.Indented);            
        }
        
        public bool Is()
        {
            string serializedData = SerializeData();
            if (_domainModel == null) return false;
            if (_serializedData == serializedData) return true;

            throw new Exception($"there was invalid modified. {Environment.NewLine}seal{Environment.NewLine}\"{_serializedData}\"{Environment.NewLine}current{Environment.NewLine}\"{serializedData}\"");
        }        

        public static Dirty Seal<T>(T domainModel) where T : class
        {
            return new Dirty(domainModel);
        }

        public static Dirty Clear()
        {
            return new Dirty();
        }
    }
}