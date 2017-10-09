using System;
using System.Collections.Generic;
using System.Text;

namespace FreestyleOrm
{
    public class TempTable
    {
        public string Columns { get; set; }
        public string IndexKeys { get; set; }
        public IEnumerable<object> Values { get; set; }
    }
}
