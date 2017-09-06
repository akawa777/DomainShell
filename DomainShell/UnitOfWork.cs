using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace DomainShell
{
    
    public abstract class UnitOfWorkFoundationBase<TDomainModel>
    {
        public void Save()
        {
            TDomainModel[] domainModels = GetOrderedDomainModels();

            foreach (TDomainModel domainModel in domainModels)
            {                
                Save(domainModel);
            }            
        }

        private TDomainModel[] GetOrderedDomainModels()
        {
            TDomainModel[] domainModels = GetTargetDomainModels();
            List<TDomainModel> deletedModels = new List<TDomainModel>();
            List<TDomainModel> modifiedModels = new List<TDomainModel>();

            Dictionary<TDomainModel, (int no, bool deleted)> domainModelMap = new Dictionary<TDomainModel, (int no, bool deleted)>();

            foreach (TDomainModel domainModel in domainModels)
            {
                Dirty dirty = GetDirty(domainModel);

                if (dirty == null || !dirty.Is)
                {
                    continue;
                }

                Deleted deleted = GetDeleted(domainModel);

                if (deleted != null && deleted.Is)
                {
                    deletedModels.Add(domainModel);
                    continue;
                }

                modifiedModels.Add(domainModel);
            }

            return deletedModels.Concat(modifiedModels).ToArray();
        }

        protected abstract Dirty GetDirty(TDomainModel domainModel);
        protected abstract Deleted GetDeleted(TDomainModel domainModel);
        protected abstract TDomainModel[] GetTargetDomainModels();        
        protected abstract void Save(TDomainModel domainModel);
    }
}
