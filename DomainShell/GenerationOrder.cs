using System;

namespace DomainShell
{    
    public interface IGenerationOrder
    {
        int GetNew();
    }

    public class GenerationOrderFoundation : IGenerationOrder
    {
        private static object _lock = new object();
        private int _no = 0;

        public int GetNew()
        {
            lock(_lock)
            {
                _no++;
                return _no;
            }
        }
    }

    public static class GenerationOrder
    {
        private static Func<IGenerationOrder> _getGenerationOrder;

        public static void Startup(Func<IGenerationOrder> getGenerationOrder)
        {
            _getGenerationOrder = getGenerationOrder;
        }

        public static int GetNew()
        {
            IGenerationOrder generationOrder = _getGenerationOrder();
            return generationOrder.GetNew();
        }
    }

    public interface IGenerationOrderGetter
    {
        int No { get;}
    }
    
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

    public sealed class Deleted : IGenerationOrderGetter
    {        
        private Deleted(bool deleted)
        {
            Is = deleted;
            _no = GenerationOrder.GetNew();
        }
        
        private int _no = 0;

        int IGenerationOrderGetter.No
        {
            get { return _no; }
        }

        public bool Is { get; private set; }

        public static Deleted True()
        {
            return new Deleted(true);
        }

        public static Deleted False()
        {
            return new Deleted(false);
        }
    }
}