using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;
using DomainShell.Test.Domains;

namespace DomainShell.Test.Infras
{
    public abstract class WriteRepository<TAggregateRoot> : IWriteRepository<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot
    {
        public void Save(TAggregateRoot model)
        {
            ModelState modelState = GetModelState(model);

            ValidateConcurrency(model, modelState);

            AdjustWhenSave(model, modelState);

            Save(model, modelState);
        }

        private ModelState GetModelState(TAggregateRoot model)
        {
            if (!model.Dirty.Is()) return ModelState.Unchanged;
            if (model.Dirty.Is() && model.Deleted) return ModelState.Deleted;
            if (model.Dirty.Is() && model.RecordVersion == 0) return ModelState.Added;
            else return ModelState.Modified;
        }

        private void ValidateConcurrency(TAggregateRoot model, ModelState modelState)
        {
            TAggregateRoot storedModel = Find(model);

            bool valid = true;

            if (modelState == ModelState.Added && storedModel != null) valid = false;
            if (modelState == ModelState.Modified && storedModel.RecordVersion != model.RecordVersion) valid = false;
            if (modelState == ModelState.Deleted && storedModel.RecordVersion != model.RecordVersion) valid = false;

            if (!valid) throw new Exception("concurrency exception.");
        }

        private void AdjustWhenSave(TAggregateRoot model, ModelState modelState)
        {
            if (modelState == ModelState.Unchanged) return;

            Surrogate<TAggregateRoot> surrogate = new Surrogate<TAggregateRoot>(model);

            surrogate
                .Set(m => m.RecordVersion, (m, p) => m.RecordVersion + 1)
                .Set(m => m.Dirty, (m, p) => Dirty.Clear(m));
        }

        protected abstract TAggregateRoot Find(TAggregateRoot model);

        protected void Save(TAggregateRoot model, ModelState modelState)
        {
            if (modelState == ModelState.Deleted)
            {
                Delete(model);
            }
            else if (modelState == ModelState.Added)
            {
                Insert(model);
            }
            else if (modelState == ModelState.Modified)
            {
                Update(model);
            }
            else
            {
                return;
            }
        }

        protected abstract void Insert(TAggregateRoot model);

        protected abstract void Update(TAggregateRoot model);

        protected abstract void Delete(TAggregateRoot model);
    }
}
