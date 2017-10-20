using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMvt.Tests.Model.Utils;

namespace SharpMvt.Tests.Model
{
    public interface IBehaviorService
    {
        string ExtendEcho(string text);
    }

    public class BehaviorService : IBehaviorService
    {
        public string ExtendEcho(string text)
        {
            return $"extend {text}";
        }
    }

    [TypeScript]    
    public class NoticeMessage
    {
        public NoticeMessage()
        {
        }

        public NoticeMessage(int defaultMessage)
        {
            _defaultMessage = defaultMessage;
        }

        public NoticeMessage([TypeScriptDi] BehaviorService service)
        {            
            _service = service;
        }

        public NoticeMessage(int defaultMessage, [TypeScriptDi] BehaviorService service)
        {
            _defaultMessage = defaultMessage;
            _service = service;
        }

        public NoticeMessage(int defaultMessage, [TypeScriptDi] BehaviorService service, string param1)
        {
            _defaultMessage = defaultMessage;
            _service = service;
        }

        public NoticeMessage(string defaultMessage, string paramA, string paramB, string paramC)
        {
            _defaultMessage = defaultMessage;
        }

        public NoticeMessage(string defaultMessage)
        {
            _defaultMessage = defaultMessage;
        }

        public NoticeMessage(Message defaultMessage)
        {
            _defaultMessage = defaultMessage.Text;
        }

        private object _defaultMessage = "hello";
        private IBehaviorService _service = null;

        public string Notice()
        {
            return _defaultMessage.ToString();
        }

        public string Notice(string message)
        {
            return message;
        }

        public bool Notice(bool message)
        {
            return true;
        }

        public string Notice(int message)
        {
            return message.ToString();
        }

        public string Notice(Message message)
        {
            return message.Text;
        }

        public Message Notice(Message message, string param1)
        {
            return message;
        }

        public bool Notice(Message message, string paramA, string paramB)
        {
            return true;
        }

        public string Notice(Message message, string paramA, string paramB, string paramC)
        {
            return message.Text;
        }        

        public Message EchoNotice(Message message)
        {
            if (_service != null)
            {
                message.Text = _service.ExtendEcho(message.Text);
            }

            return message;
        }

        [SharpMvt.TypeScriptMethod(Form = true)]
        public System.IO.Stream GetStream(System.IO.Stream stream)
        {
            return null;
        }

        [SharpMvt.TypeScriptMethod(Link = true)]
        public System.IO.Stream GetImage(System.IO.Stream stream)
        {
            return null;
        }

        public Message[] GetArray(string[] array)
        {
            return null;
        }

        public Message[] GetArray(Message[] array)
        {
            return null;
        }

        public List<Message> GetList(List<Message> array)
        {
            return null;
        }

        public Dictionary<string, string> GetList()
        {
            return null;
        }
    }
}
