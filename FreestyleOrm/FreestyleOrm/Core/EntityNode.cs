using System;
using System.Collections.Generic;
using System.Text;

namespace FreestyleOrm.Core
{
    internal class EntityNode : IEntityNode
    {
        public object Entity { get; set; }

        public IEntityNode Child { get; set; }
    }

    internal class RootEntityNode : EntityNode, IRootEntityNode
    {

    }
}
