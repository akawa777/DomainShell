using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.App
{
    public interface IApp
    {

    }

    public interface IApp<TRequest> : IApp
    {
        void Execute(TRequest request);
    }

    public interface IApp<TRequest, TResult> : IApp
    {
        TResult Execute(TRequest request);
    }
}
