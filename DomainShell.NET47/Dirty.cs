using System;
using Newtonsoft.Json;

namespace DomainShell
{    
    public struct Dirty
    {
        private Dirty(object domainModel, bool isClear = false)
        {
            if (isClear)
            {
                _domainModel = null;
                _serializedData = null;
            }
            else
            {
                _domainModel = domainModel;
                _serializedData = SerializeData(_domainModel);

                DomainModelTracker.Mark(domainModel);
            }
        }

        private object _domainModel;
        private string _serializedData;
        
        private static string SerializeData(object domainMpdel)
        {
            return JsonConvert.SerializeObject(domainMpdel, Formatting.Indented);            
        }
        
        public bool Verify()
        {
            if (_domainModel == null) return false;

            string serializedData = SerializeData(_domainModel);
            if (_serializedData == serializedData) return true;

            throw new InvalidOperationException($"there was invalid modified. {Environment.NewLine}seal{Environment.NewLine}\"{_serializedData}\"{Environment.NewLine}current{Environment.NewLine}\"{serializedData}\"");
        }        

        public static Dirty Seal<T>(T domainModel) where T : class
        {
            return new Dirty(domainModel);
        }

        public static Dirty Clear<T>(T domainModel) where T : class
        {
            return new Dirty(domainModel, isClear: true);
        }
    }
}