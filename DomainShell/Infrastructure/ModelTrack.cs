using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Linq.Expressions;

namespace DomainShell.Infrastructure
{
    public enum ModelState
    {
        UnChanged,
        Added,
        Modified,
        Deleted
    }

    public class ModelTrack<TModel>
    {
        public ModelTrack(TModel model, Expression<Func<TModel, object>> trackingTarget, ModelState state)
        {
            _model = model;
            _target = trackingTarget.Compile()(_model);
            _state = state;
            
            _serializer = new DataContractJsonSerializer(_target.GetType());
            
            _graph = GetGraph(_target);
        }

        private TModel _model;
        private object _target;
        private ModelState _state;        
        private DataContractJsonSerializer _serializer;
        
        private string _graph;
        
        private string GetGraph(object traget)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                _serializer.WriteObject(stream, traget);                                
                stream.Position = 0;
                return new StreamReader(stream).ReadToEnd();
            }

        }

        public TModel Model
        {
            get 
            { 
                return _model;  
            }
        }

        public ModelState State 
        { 
            get
            {                
                string newGraph = GetGraph(_target);             
             
                if (_state == ModelState.UnChanged && _graph != newGraph)
                {
                    _graph = newGraph;

                    _state = ModelState.Modified;
                }

                return _state;
            }
        }

        public void AcceptChanges()
        {
            _state = ModelState.UnChanged;
        }
    }
}
