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
            _graph = GetCurrentGraph();
            
            DomainModelMarker.Mark(domainModel);
        }

        private object _domainModel;
        private string _graph;
        
        private string GetCurrentGraph()
        {
            return JsonConvert.SerializeObject(_domainModel, Formatting.Indented);            
        }
        
        public bool Is()
        {
            string currentGraph = GetCurrentGraph();
            if (_domainModel == null) return false;
            if (_graph == currentGraph) return true;

            throw new Exception($"there was invalid modified. {Environment.NewLine}seal{Environment.NewLine}\"{_graph}\"{Environment.NewLine}current{Environment.NewLine}\"{currentGraph}\"");
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