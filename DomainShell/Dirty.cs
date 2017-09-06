using System;

namespace DomainShell
{    
    public sealed class Dirty
    {
        private Dirty(bool isDirty)
        {
            Is = isDirty;            
        }

        private Dirty(bool isDirty, object domainModel)
        {
            Is = isDirty;
            DomainModelMarker.Mark(domainModel);
        }

        public bool Is { get; private set; }

        public static Dirty True<T>(T domainModel) where T : class
        {
            return new Dirty(true, domainModel);
        }

        public static Dirty False()
        {
            return new Dirty(false);
        }
    }
}