using System;
using Newtonsoft.Json;

namespace DomainShell
{    
    public struct ModelState
    {
        private ModelState(object domainModel, bool isClear = false)
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
            }
        }

        private object _domainModel;
        private string _serializedData;
        
        private static string SerializeData(object domainMpdel)
        {
            return JsonConvert.SerializeObject(domainMpdel, Formatting.Indented);            
        }
        
        public bool Sealed()
        {
            if (_domainModel == null) return false;
            
            string serializedData = SerializeData(_domainModel);
            if (_serializedData == serializedData) true;

            throw new InvalidOperationException($"there was invalid modified. {Environment.NewLine}seal{Environment.NewLine}\"{_serializedData}\"{Environment.NewLine}current{Environment.NewLine}\"{serializedData}\"");
        }        

        public static ModelState Seal<T>(T domainModel) where T : class
        {
            return new ModelState(domainModel);
        }
    }
}