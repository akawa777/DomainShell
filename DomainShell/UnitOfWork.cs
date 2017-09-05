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
            Dictionary<TDomainModel, (int no, bool deleted)> domainModelMap = new Dictionary<TDomainModel, (int no, bool deleted)>();

            foreach (TDomainModel domainModel in domainModels)
            {
                Deleted deleted = GetDeleted(domainModel);

                if (deleted != null && deleted.Is)
                {
                    domainModelMap[domainModel] = ((deleted as IGenerationOrderGetter).No, true);
                    continue;
                }

                Dirty dirty = GetDirty(domainModel);

                if (dirty != null && dirty.Is)
                {
                    domainModelMap[domainModel] = ((dirty as IGenerationOrderGetter).No, false);
                }
            }

            return domainModelMap.OrderBy(x => x.Value.no).Select(x => x.Key).ToArray();
        }

        protected abstract Dirty GetDirty(TDomainModel domainModel);
        protected abstract Deleted GetDeleted(TDomainModel domainModel);
        protected abstract TDomainModel[] GetTargetDomainModels();        
        protected abstract void Save(TDomainModel domainModel);
    }
}
