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
    public interface ILogKernel : IList
    {
        string[] Messages { get; }
        void SetMessage(string message);
    }

    public class LogKernel : List<string>, ILogKernel
    {
        private List<string> _messages = new List<string>();

        public string[] Messages => _messages.ToArray();

        public void SetMessage(string message)
        {
            _messages.Add(message);
        }
    }
}