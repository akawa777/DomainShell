using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Specialized;
using DomainShell.Kernels;

namespace DomainShell.Test
{
    public static class Log
    {
        private static Action<string> _handle = x => { };

        public static void SetMessage(string message)
        {
            _messageList.Add(message);
            _handle(message);
        }

        public static void HandleMessage(Action<string> handle)
        {
            _handle = handle;
        }

        private static List<string> _messageList { get; } = new List<string>();
        public static string[] MessageList => _messageList.ToArray();
    }
}