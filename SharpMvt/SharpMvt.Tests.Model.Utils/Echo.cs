using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMvt.Tests.Model.Utils
{
    [SharpMvt.TypeScript]
    public class Echo
    {
        public Echo(bool init)
        {

        }

        public string Do(Message message)
        {
            return message.Text;
        }

        public string Do(string message)
        {
            return message;
        }

        public Message Do(Message message, bool can)
        {
            return message;
        }
    }
}
