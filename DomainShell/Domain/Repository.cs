using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Domain
{
    public interface IReadReposiory<TAggregateRoot, TIdentity>
    {
        TAggregateRoot Find(TIdentity id);
    }

    public interface IReadSpecReposiory<TTarget>
    {
        IEnumerable<TTarget> List(ISpecification<TTarget> spec);
    }

    public interface IReadSpecReposiory<TTarget, TPredicate>
    {
        IEnumerable<TTarget> List(ISelectionSpec<TTarget, TPredicate> spec); 
    }

    public interface IReadPredicateSpecReposiory<TTarget, TOperator>
    {
        IEnumerable<TTarget> List(ISelectionPredicateSpec<TTarget, TOperator> spec);
    }

    public interface IWriteRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot
    {        
        void Save(TAggregateRoot aggregateRoot);
    }

    public interface IWriteCollectionRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot
    {
        void Add(TAggregateRoot aggregateRoot);
        void Remove(TAggregateRoot aggregateRoot);
        void Save();
    }
}

namespace DomainShell.DomainProto
{
    public interface IDomainEvent
    {

    }

    public interface IDomainEvnetPublisher
    {
        void Publish(IDomainEvent domainEvent);
    }

    public interface IAggregateRoot
    {
        bool Stored { get; }
        bool Deleted { get; }
        void Accepted();
        IDomainEvent[] GetEvents();
        void ClearEvents();
    }

    public interface ISelectItem
    {
        object Value { get; }
        object[] Values { get; }
        bool AndOr { get; set; }
        ISelectItem[] Nodes { get; }
    }

    public class SelectItem : ISelectItem
    {
        public object Value
        {
            get;
            protected set;
        }

        public object[] Values
        {
            get;
            protected set;
        }

        public bool AndOr
        {
            get;
            set;
        }

        public ISelectItem[] Nodes
        {
            get;
            set;
        }
    }

    public interface ISortItem
    {
        bool Desc { get; set; }
    }

    public interface ISelectionSpec<TSelect, TSort> where  TSelect : ISelectItem where TSort : ISortItem
    {
        ISelectItem[] GetSelectItems();
        ISortItem[] GetSortItems();
        ISelectionSpec<TSelect, TSort> And(TSelect select);
        ISelectionSpec<TSelect, TSort> Or(TSelect select);
        ISelectionSpec<TSelect, TSort> And(ISelectionSpec<TSelect, TSort> spec);
        ISelectionSpec<TSelect, TSort> Or(ISelectionSpec<TSelect, TSort> spec);
        ISelectionSpec<TSelect, TSort> Sort(params TSort[] sorts);
    }

    public class SelectionSpec<TSelect, TSort> : ISelectionSpec<TSelect, TSort> where  TSelect : ISelectItem where TSort : ISortItem
    {
        public SelectionSpec(TSelect select)
        {
            select.AndOr = true;
            _selectItems = new ISelectItem[] { select };
        }

        protected SelectionSpec(ISelectItem[] selectItems, params ISortItem[] sortItems)
        {
            _selectItems = selectItems as ISelectItem[];
            _sortItems = sortItems as ISortItem[];
        }

        private ISelectItem[] _selectItems;
        private ISortItem[] _sortItems = new ISortItem[0];

        public ISelectItem[] GetSelectItems()
        {
            return _selectItems;
        }

        public ISortItem[] GetSortItems()
        {
            return _sortItems;
        }

        public ISelectionSpec<TSelect, TSort> And(TSelect select)
        {
            select.AndOr = true;

            List<ISelectItem> cloneSelectItems = new List<ISelectItem>(_selectItems);
            cloneSelectItems.Add(select);

            return new SelectionSpec<TSelect, TSort>(cloneSelectItems.ToArray());
        }

        public ISelectionSpec<TSelect, TSort> Or(TSelect select)
        {
            select.AndOr = false;

            List<ISelectItem> cloneSelectItems = new List<ISelectItem>(_selectItems);
            cloneSelectItems.Add(select);

            return new SelectionSpec<TSelect, TSort>(cloneSelectItems.ToArray());
        }

        public ISelectionSpec<TSelect, TSort> And(ISelectionSpec<TSelect, TSort> spec)
        {
            List<ISelectItem> cloneSelectItems = new List<ISelectItem>(_selectItems);

            SelectItem selectItem = new SelectItem { AndOr = true, Nodes = spec.GetSelectItems() };

            cloneSelectItems.Add(selectItem);

            return new SelectionSpec<TSelect, TSort>(cloneSelectItems.ToArray());
        }

        public ISelectionSpec<TSelect, TSort> Or(ISelectionSpec<TSelect, TSort> spec)
        {
            List<ISelectItem> cloneSelectItems = new List<ISelectItem>(_selectItems);

            SelectItem selectItem = new SelectItem { AndOr = false, Nodes = spec.GetSelectItems() };

            cloneSelectItems.Add(selectItem);

            return new SelectionSpec<TSelect, TSort>(cloneSelectItems.ToArray());
        }

        public ISelectionSpec<TSelect, TSort> Sort(params TSort[] sortItems)
        {
            List<ISelectItem> cloneSelectItems = new List<ISelectItem>(_selectItems);            
            return new SelectionSpec<TSelect, TSort>(cloneSelectItems.ToArray(), sortItems as ISortItem[]);
        }
    }

    public interface IReadReposiory<TModel, TSelect, TSort> where TSelect : ISelectItem where TSort : ISortItem
    {
        TModel Single(ISelectionSpec<TSelect, TSort> spec);
        IEnumerable<TModel> List(ISelectionSpec<TSelect, TSort> spec);
    }

    public interface IWriteRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot
    {
        void Save(TAggregateRoot model);
    }

    public abstract class AggregateRoot : IAggregateRoot
    {
        public AggregateRoot()
        {
            DomainEvent = new List<IDomainEvent>();
        }

        protected virtual List<IDomainEvent> DomainEvent { get; set; }

        public virtual bool Stored
        {
            get;
            protected set;
        }

        public virtual bool Deleted
        {
            get;
            protected set;
        }

        public virtual void Accepted()
        {
            if (Deleted)
            {
                Stored = false;
            }
            else
            {
                Stored = true;
            }
        }

        public virtual IDomainEvent[] GetEvents()
        {
            return DomainEvent.ToArray();
        }

        public virtual void ClearEvents()
        {
            DomainEvent.Clear();
        }
    }

    public abstract class ReadSqlReposiory<TModel, TSelect, TSort> : IReadReposiory<TModel, TSelect, TSort>
        where TSelect : ISelectItem
        where TSort : ISortItem
    {
        public TModel Single(ISelectionSpec<TSelect, TSort> spec)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TModel> List(ISelectionSpec<TSelect, TSort> spec)
        {
            throw new NotImplementedException();
        }

        protected virtual string GetWhere(ISelectItem[] selectItems, ref int parameterNo, Dictionary<string, object> parameters, StringBuilder preSql)
        {
            StringBuilder where = new StringBuilder();

            foreach (ISelectItem selectItem in selectItems)
            {
                if (where.ToString() != string.Empty)
                {
                    if (selectItem.AndOr)
                    {
                        where.Append(" and ");
                    }
                    else
                    {
                        where.Append(" or ");
                    }
                }

                if (selectItem.Nodes.Length == 0)
                {
                    string whereSelectItem = GetWhereItem(selectItem, parameterNo, parameters, preSql);
                    where.Append(whereSelectItem);
                    parameterNo++;
                }
                else
                {
                    where.Append(" ( ");

                    string nodesWhere = GetWhere(selectItem.Nodes, ref parameterNo, parameters, preSql);

                    where.Append(nodesWhere);

                    where.Append(" ) ");
                }
            }

            return where.ToString();
        }

        protected abstract string GetWhereItem(ISelectItem selectItem, int parameterNo, Dictionary<string, object> parameters, StringBuilder preSql);
        protected abstract string GetOrderItem(ISortItem sortItem);
    }

    public abstract class WriteRepository<TAggregateRoot> : IWriteRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot
    {
        public WriteRepository(IDomainEvnetPublisher domainEvnetPublisher)
        {
            DomainEvnetPublisher = domainEvnetPublisher;
        }

        protected virtual IDomainEvnetPublisher DomainEvnetPublisher { get; set; }

        public virtual void Save(TAggregateRoot model)
        {
            if (model.Deleted)
            {
                Delete(model);
            }
            else if (model.Stored)
            {
                Update(model);
            }
            else
            {
                Create(model);
            }

            IDomainEvent[] domainEvents = model.GetEvents();
            model.ClearEvents();

            foreach (IDomainEvent domainEvent in domainEvents)
            {
                DomainEvnetPublisher.Publish(domainEvent);
            }
        }

        protected abstract void Create(TAggregateRoot model);
        protected abstract void Update(TAggregateRoot model);
        protected abstract void Delete(TAggregateRoot model);
    }

    public abstract class SqlRepository<TAggregateRoot, TSelect, TSort> : ReadSqlReposiory<TAggregateRoot, TSelect, TSort>, IWriteRepository<TAggregateRoot> 
        where TSelect : ISelectItem
        where TSort : ISortItem
        where TAggregateRoot : IAggregateRoot
    {
        public SqlRepository(IDomainEvnetPublisher domainEvnetPublisher)
        {
            DomainEvnetPublisher = domainEvnetPublisher;
        }

        protected virtual IDomainEvnetPublisher DomainEvnetPublisher { get; set; }

        public virtual void Save(TAggregateRoot model)
        {
            if (model.Deleted)
            {
                Delete(model);
            }
            else if (model.Stored)
            {
                Update(model);
            }
            else
            {
                Create(model);
            }

            IDomainEvent[] domainEvents = model.GetEvents();
            model.ClearEvents();

            foreach (IDomainEvent domainEvent in domainEvents)
            {
                DomainEvnetPublisher.Publish(domainEvent);
            }
        }

        protected abstract void Create(TAggregateRoot model);
        protected abstract void Update(TAggregateRoot model);
        protected abstract void Delete(TAggregateRoot model);
    }

    public class Customer : AggregateRoot
    {
        public string CustomerId { get; set;}
        public string CustomerName { get; set;}
    }

    public class CustomerSelectItem : ISelectItem
    {
        public object Value
        {
            get;
            protected set;
        }

        public object[] Values
        {
            get;
            protected set;
        }

        public bool AndOr
        {
            get;
            set;
        }

        public ISelectItem[] Nodes
        {
            get;
            protected set;
        }

        public class CustomerId : CustomerSelectItem
        {            
            public CustomerId(params string[] customerIds)
            {
                Values = customerIds;
            }
        }

        public class LikeCustomerName : CustomerSelectItem
        {
            public LikeCustomerName(string customerName)
            {
                Value = customerName;
            }
        }
    }

    public class CustomerSortItem : ISortItem
    {
        public bool Desc
        {
            get;
            set;
        }

        public class CustomerId : CustomerSortItem
        {            
        }

        public class CustomerName : CustomerSortItem
        {
        }        
    }

    public class CustomerRepository : SqlRepository<Customer, CustomerSelectItem, CustomerSortItem>
    {
        public CustomerRepository(IDomainEvnetPublisher domainEvnetPublisher)
            : base(domainEvnetPublisher)
        {
            
        }

        protected override string GetWhereItem(ISelectItem selectItem, int parameterNo, Dictionary<string, object> parameters, StringBuilder preSql)
        {
            if (selectItem is CustomerSelectItem.CustomerId)
            {
                preSql.Append(string.Format(@"
                    create table #CustomorIds_{0} ( 
                        CustomerId nvarchar(32) not null primary key 
                    )",
                    parameterNo));

                if (selectItem.Values != null)
                {
                    foreach (object value in selectItem.Values)
                    {
                        preSql.Append(string.Format(@"
                            insert into #customorId_{0} values('{1}')",
                            parameterNo,
                            value.ToString()));
                    }
                }

                return string.Format("CustomerId in (select CustomerId from #CustomorIds_{0})", parameterNo);
            }
            else if (selectItem is CustomerSelectItem.LikeCustomerName)
            {
                string parameterName = string.Format("@LikeCustomerName_{0}%", parameterNo);
                parameters[parameterName] = selectItem.Value;
                return string.Format("CustomerName like {0}%", parameterName);
            }
            else
            {
                return string.Empty;
            }
        }

        protected override string GetOrderItem(ISortItem sortItem)
        {
            if (sortItem is CustomerSortItem.CustomerId)
            {
                return "CustomerId";
            }
            else if (sortItem is CustomerSortItem.CustomerName)
            {
                return "CustomerName";
            }
            else
            {
                return string.Empty;
            }
        }

        protected override void Create(Customer model)
        {
            throw new NotImplementedException();
        }

        protected override void Update(Customer model)
        {
            throw new NotImplementedException();
        }

        protected override void Delete(Customer model)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomerApp
    {
        public CustomerApp()
        {
            _repository = new CustomerRepository(_domainEvnetPublisher);
        }

        private IDomainEvnetPublisher _domainEvnetPublisher = null;
        private CustomerRepository _repository;

        public object[] List(string[] customerIds, string likeCustomerName)
        {
            SelectionSpec<CustomerSelectItem, CustomerSortItem> spec
                = new SelectionSpec<CustomerSelectItem, CustomerSortItem>(new CustomerSelectItem.CustomerId(customerIds));

            spec.And(new CustomerSelectItem.LikeCustomerName(likeCustomerName));

            IEnumerable<Customer> models = _repository.List(spec);

            return models.ToArray();
        }

        public object[] List(string[] customerIds1, string[] customerIds2)
        {
            SelectionSpec<CustomerSelectItem, CustomerSortItem> spec1
                = new SelectionSpec<CustomerSelectItem, CustomerSortItem>(new CustomerSelectItem.CustomerId(customerIds1));

            SelectionSpec<CustomerSelectItem, CustomerSortItem> spec2
                = new SelectionSpec<CustomerSelectItem, CustomerSortItem>(new CustomerSelectItem.CustomerId(customerIds2));

            ISelectionSpec<CustomerSelectItem, CustomerSortItem> mergeSpec = spec1.And(spec2);

            IEnumerable<Customer> models = _repository.List(mergeSpec);

            return models.ToArray();
        }
    }
}
