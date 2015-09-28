using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Message
{
    public interface IMessage
    {

    }

    public interface IMessage<TResult> : IMessage
    {

    }

    public interface IAsyinc
    {
        bool DoAsyinc();
    }

    public interface IMessageResult
    {
        void Set<TMessage, TResult>(TMessage message, TResult result) where TMessage : IMessage<TResult>;
        void Clear<TResult>(IMessage<TResult> message);
        TResult Get<TResult>(IMessage<TResult> message);
    }

    public class MessageResult : IMessageResult
    {
        private Dictionary<object, object> _resultCache = new Dictionary<object, object>();

        void IMessageResult.Set<TMessage, TResult>(TMessage message, TResult result)
        {
            _resultCache[message] = result;
        }

        void IMessageResult.Clear<TResult>(IMessage<TResult> message)
        {
            _resultCache.Remove(message);
        }

        TResult IMessageResult.Get<TResult>(IMessage<TResult> message)
        {
            object result;

            if (_resultCache.TryGetValue(message, out result))
            {
                _resultCache.Remove(message);
                return (TResult)result;
            }
            else
            {
                return default(TResult);
            }
        }
    }

    public interface IMessageHandler
    {
        
    }

    public interface IMessageHandler<TMessage> : IMessageHandler where TMessage : IMessage
    {

    }

    public class MessagePublisher
    {
        protected Dictionary<Type, List<Func<object>>> _handlerMap = new Dictionary<Type, List<Func<object>>>();
        protected Dictionary<IMessage, dynamic> _callbackCache = new Dictionary<IMessage, dynamic>();

        public void Register<TMessage>(Func<IMessageHandler<TMessage>> handler) where TMessage : IMessage
        {
            if (!_handlerMap.ContainsKey(typeof(TMessage)))
            {
                _handlerMap[typeof(TMessage)] = new List<Func<object>>();
            }

            _handlerMap[typeof(TMessage)].Add(handler);
        }

        public void Callback<TResult>(IMessage<TResult> message, Action<TResult> action)
        {
            _callbackCache[message] = action as dynamic;
        }

        public void Publish<TMessage>(TMessage message, Func<IMessageHandler<TMessage>, IMessageResult> getMessageResult) where TMessage : IMessage
        {
            List<Func<object>> handlers;

            if (_handlerMap.TryGetValue(typeof(TMessage), out handlers))
            {
                foreach (Func<object> handlerFunc in handlers)
                {
                    IMessageHandler<TMessage> handler = handlerFunc() as IMessageHandler<TMessage>;

                    try
                    {
                        _beginHandle(message, handler);

                        Handle(message, handler, getMessageResult);                        

                        _endHandle(message, handler, null);
                    }
                    catch (Exception e)
                    {
                        _callbackCache.Remove(message);

                        _endHandle(message, handler, e);
                        
                    }
                }

                _callbackCache.Remove(message);
            }
            else
            {
                throw new Exception("not regist domain event handler");
            }
        }

        private void Handle<TMessage>(TMessage message, IMessageHandler<TMessage> handler, Func<IMessageHandler<TMessage>, IMessageResult> getMessageResult) where TMessage : IMessage
        {
            Action handleAction = () =>
            {   
                (handler as dynamic).Handle(message);

                IMessageResult result = getMessageResult(handler);

                dynamic callback;

                if (_callbackCache.TryGetValue(message, out callback))
                {                    
                    callback(result.Get(message as dynamic));
                }

                result.Clear(message as dynamic);
            };

            if (message is IAsyinc && (message as IAsyinc).DoAsyinc())
            {
                Task.Run(() => handleAction());
            }
            else
            {
                handleAction();
            }
        }

        private Action<IMessage, IMessageHandler> _beginHandle = (message, handler) => { };
        private Action<IMessage, IMessageHandler, Exception> _endHandle = (message, handler, exception) => { };

        public void SetBeginHandle(Action<IMessage, IMessageHandler> beginHandle)
        {
            _beginHandle = beginHandle;
        }

        public void SetEndHandle(Action<IMessage, IMessageHandler, Exception> endHandle)
        {
            _endHandle = endHandle;
        }
    }   
}
