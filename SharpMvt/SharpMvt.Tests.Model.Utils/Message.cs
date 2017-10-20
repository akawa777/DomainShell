using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMvt.Tests.Model.Utils
{   
    public class Message
    {
        public string Text { get; set; }
        public string[] Errors { get; set; }
        public List<Info> InfoList { get; set; }
    }

    public class Info
    {
        public List<string> TextList { get; set; }
    }
}
