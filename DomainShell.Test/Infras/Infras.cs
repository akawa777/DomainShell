using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;
using System.Data.Common;
using DomainShell.Test.Domains;
using DomainShell.Test.Domains.Entites;
using Microsoft.EntityFrameworkCore;

namespace DomainShell.Test.Infras
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(IDbConnection connection)
        {
            _connection = connection as DbConnection;
        }
        public DatabaseContext(IConnection connection)
        {
            _connection = connection.CreateCommand().Connection as DbConnection;
        }
        private DbConnection _connection;
         public DbSet<LoginUser> LoginUser { get; set; }
        // public DbSet<OrderForm> OrderForms { get; set; }
        // public DbSet<OrderFormCanceled> OrderFormCanceleds { get; set;}
        public DbSet<MonthlyOrderBudget> MonthlyOrderBudget { get; set;}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {   
            optionsBuilder.UseSqlite(_connection);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginUser>().HasKey(x => x.UserId);            
            modelBuilder.Entity<MonthlyOrderBudget>().HasKey(x => x.UserId);            
        }
    }

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
            bool dirty = model.Dirty.Verify();

            if (!dirty) return ModelState.Unchanged;
            if (model.Deleted) return ModelState.Deleted;
            if (model.RecordVersion == 0) return ModelState.Added;
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

            ProxyObject<TAggregateRoot> ProxyObject = new ProxyObject<TAggregateRoot>(model);

            ProxyObject
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
