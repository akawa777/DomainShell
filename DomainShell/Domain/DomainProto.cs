using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.DomainProto
{
    public interface IDomainEvent
    {
        bool Async { get; }
    }

    public interface IDomainEventHandler<TDomainEvent> 
        where TDomainEvent : IDomainEvent
    {
        void Handle(TDomainEvent domainEvent);
    }

    public interface IDomainEvnetPublisher
    {
        void Publish(IDomainEvent domainEvent);        
    }

    public interface IAggregateRoot
    {
        IDomainEvent[] GetEvents();
        void ClearEvents();        
    }    

    public interface IProxyAggregateRoot
    {
        bool New { get; set; }
        bool Deleted { get; set; }
        bool Verified { get; set; }
    }

    public interface IFactory<TAggregateRoot, TCreationSpec> 
        where TAggregateRoot : IAggregateRoot
    {
        TAggregateRoot Create(TCreationSpec spec);
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
        public virtual object Value
        {
            get;
            protected set;
        }

        public virtual object[] Values
        {
            get;
            protected set;
        }

        public virtual bool AndOr
        {
            get;
            set;
        }

        public virtual ISelectItem[] Nodes
        {
            get;
            set;
        }
    }

    public interface ISortItem
    {
        bool Desc { get; }
    }

    public class SortItem : ISortItem
    {
        public virtual bool Desc { get; set; }
    }

    public interface ISelectionSpec<TSelect, TSort> 
        where TSelect : ISelectItem 
        where TSort : ISortItem
    {
        ISelectItem[] GetSelectItems();
        ISortItem[] GetSortItems();
        ISelectionSpec<TSelect, TSort> And(TSelect select);
        ISelectionSpec<TSelect, TSort> Or(TSelect select);
        ISelectionSpec<TSelect, TSort> And(ISelectionSpec<TSelect, TSort> spec);
        ISelectionSpec<TSelect, TSort> Or(ISelectionSpec<TSelect, TSort> spec);
        ISelectionSpec<TSelect, TSort> Sort(params TSort[] sorts);
    }

    public class SelectionSpec<TSelect, TSort> : ISelectionSpec<TSelect, TSort> 
        where TSelect : ISelectItem 
        where TSort : ISortItem
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

    public interface IReadReposiory<TModel, TSelect, TSort> 
        where TSelect : ISelectItem 
        where TSort : ISortItem
    {
        TModel Single(ISelectionSpec<TSelect, TSort> spec);
        IEnumerable<TModel> List(ISelectionSpec<TSelect, TSort> spec);
    }

    public interface IWriteRepository<TAggregateRoot> 
        where TAggregateRoot : IAggregateRoot
    {
        void Save(TAggregateRoot model);
    }

    public interface IRepository<TAggregateRoot, TSelect, TSort> : IReadReposiory<TAggregateRoot, TSelect, TSort>, IWriteRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot
        where TSelect : ISelectItem
        where TSort : ISortItem
    {

    }

    public abstract class AggregateRoot : IAggregateRoot
    {
        public AggregateRoot()
        {
            DomainEvent = new List<IDomainEvent>();
        }

        protected virtual List<IDomainEvent> DomainEvent { get; set; }        

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
            IEnumerable<TModel> list = List(spec);

            return list.FirstOrDefault();
        }

        public IEnumerable<TModel> List(ISelectionSpec<TSelect, TSort> spec)
        {
            int parameterNo = 1;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            StringBuilder preSql = new StringBuilder();

            StringBuilder sql = new StringBuilder();
            sql.Append(GetSelectSql());

            if (spec.GetSelectItems().Length > 0)
            {
                string where = GetWhereSql(spec.GetSelectItems(), ref parameterNo, parameters, preSql);
                
                sql = new StringBuilder(preSql.ToString() + Environment.NewLine + sql.ToString() + Environment.NewLine + "where" + Environment.NewLine + where);
            }

            if (spec.GetSortItems().Length > 0)
            {
                StringBuilder order = new StringBuilder();

                order.Append(Environment.NewLine + "order by");

                foreach (ISortItem sortItem in spec.GetSortItems())
                {
                    string orderColumn = GetOrderItemSql(sortItem);

                    orderColumn = sortItem.Desc ? " desc" : orderColumn;

                    order.Append(Environment.NewLine + orderColumn);
                }

                sql.Append(order.ToString());
            }

            return List(sql.ToString(), parameters);
        }        

        protected virtual string GetWhereSql(ISelectItem[] selectItems, ref int parameterNo, Dictionary<string, object> parameters, StringBuilder preSql)
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
                    string whereSelectItem = GetWhereItemSql(selectItem, parameterNo, parameters, preSql);
                    where.Append(whereSelectItem);
                    parameterNo++;
                }
                else
                {
                    where.Append(" ( ");

                    string nodesWhere = GetWhereSql(selectItem.Nodes, ref parameterNo, parameters, preSql);

                    where.Append(nodesWhere);

                    where.Append(" ) ");
                }
            }

            return where.ToString();
        }

        protected abstract string GetSelectSql();
        protected abstract string GetWhereItemSql(ISelectItem selectItem, int parameterNo, Dictionary<string, object> parameters, StringBuilder preSql);
        protected abstract string GetOrderItemSql(ISortItem sortItem);
        protected abstract IEnumerable<TModel> List(string sql, Dictionary<string, object> parameters);
    }

    public abstract class SqlRepository<TAggregateRoot, TSelect, TSort> : ReadSqlReposiory<TAggregateRoot, TSelect, TSort>, IRepository<TAggregateRoot, TSelect, TSort>
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
            if (!(model is IProxyAggregateRoot))
            {
                throw new Exception(string.Format("{0} is not IProxyAggregateRoot", model.GetType().Name));
            }

            IProxyAggregateRoot proxyModel = model as IProxyAggregateRoot;            

            if (!proxyModel.Deleted && !proxyModel.Verified)
            {
                throw new Exception(string.Format("{0} is not verified", typeof(TAggregateRoot).Name));
            }

            if (proxyModel.New && !proxyModel.Deleted)
            {
                Create(model);
                proxyModel.New = false;
            }
            else if (!proxyModel.New && !proxyModel.Deleted)
            {
                Update(model);
            }
            else if (!proxyModel.New && proxyModel.Deleted)
            {
                Delete(model);
            }

            proxyModel.Verified = false;

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
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }

        public virtual void Delete()
        {

        }

        public virtual bool Validate(out string[] errors)
        {
            errors = new string[0];
            return true;
        }

        public virtual CustomerHistory[] CustomerHistoryList
        {
            get { return _customerHistoryList.ToArray();  }
        }

        protected List<CustomerHistory> _customerHistoryList = new List<CustomerHistory>();

        public CustomerHistory NewHistory()
        {
            CustomerHistory history = new CustomerHistory();
            history.CustomerId = CustomerId;
            history.HistoryNo = GetMaxHistoryNo();

            return history;
        }

        public int GetMaxHistoryNo()
        {
            int maxNo = _customerHistoryList.Count == 0 ? 1 : _customerHistoryList.Max(x => x.HistoryNo) + 1;

            return maxNo;
        }

        public virtual void AddHistory(CustomerHistory history)
        {
            _customerHistoryList.Add(history);
        }

        public virtual void RemoveHistory(CustomerHistory history)
        {
            _customerHistoryList.Remove(history);
        }
    }

    public class CustomerHistory
    {
        public string CustomerId { get; set; }
        public int HistoryNo { get; set; }
        public string Content { get; set; }

        protected List<CustomerHistoryDetail> _customerHistoryDetailList = new List<CustomerHistoryDetail>();

        public virtual CustomerHistoryDetail[] CustomerHistoryDetailList
        {
            get { return _customerHistoryDetailList.ToArray(); }
        }

        public CustomerHistoryDetail NewDetail()
        {
            CustomerHistoryDetail detail = new CustomerHistoryDetail();
            detail.CustomerId = CustomerId;
            detail.HistoryNo = HistoryNo;
            detail.DetailNo = GetMaxDetailNo();

            return detail;
        }

        public int GetMaxDetailNo()
        {
            int maxNo = _customerHistoryDetailList.Count == 0 ? 1 : _customerHistoryDetailList.Max(x => x.DetailNo) + 1;

            return maxNo;
        }

        public virtual void AddDetail(CustomerHistoryDetail detail)
        {
            _customerHistoryDetailList.Add(detail);
        }

        public virtual void RemoveDetail(CustomerHistoryDetail detail)
        {
            _customerHistoryDetailList.Remove(detail);
        }
    }

    public class CustomerHistoryDetail
    {
        public string CustomerId { get; set; }
        public int HistoryNo { get; set; }
        public int DetailNo { get; set; }
        public string Text { get; set; }
    }

    public class CustomerProxy : Customer, IProxyAggregateRoot
    {
        public CustomerProxy()
        {
            DeletedHistoryList = new List<CustomerHistory>();
        }

        public virtual bool New
        {
            get;
            set;
        }

        public virtual bool Deleted
        {
            get;
            set;
        }

        public virtual bool Verified
        {
            get;
            set;
        }

        public override void Delete()
        {
            base.Delete();
            Deleted = true;
        }

        public override bool Validate(out string[] errors)
        {
            bool validate = base.Validate(out errors);
            Verified = validate ? true : false;
            return validate;
        }

        public new List<CustomerHistory> CustomerHistoryList 
        {
            get
            {
                return _customerHistoryList;
            }
            set
            {
                _customerHistoryList = value;
            }
        }

        public List<CustomerHistory> DeletedHistoryList { get; set; }

        public override void RemoveHistory(CustomerHistory history)
        {   
            DeletedHistoryList.Add(history);

            base.RemoveHistory(history);
        }
    }

    public class CustomerHistoryProxy : CustomerHistory
    {
        public new List<CustomerHistoryDetail> CustomerHistoryDetailList
        {
            get
            {
                return _customerHistoryDetailList;
            }
            set
            {
                _customerHistoryDetailList = value;
            }
        }
    }

    public class CustomerCreationSpec
    {
        public string CustomerName { get; set; }
    }

    public class CustomerFactory : IFactory<Customer, CustomerCreationSpec>
    {
        public Customer Create(CustomerCreationSpec spec)
        {
            CustomerProxy proxyModel = new CustomerProxy();

            proxyModel.CustomerId = Guid.NewGuid().ToString();
            proxyModel.CustomerName = spec.CustomerName;
            proxyModel.New = true;

            return proxyModel;
        }
    }

    public class CustomerSelectItem : SelectItem
    {
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

    public class CustomerSortItem : SortItem
    {
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

        protected override string GetSelectSql()
        {
            throw new NotImplementedException();
        }

        protected override string GetWhereItemSql(ISelectItem selectItem, int parameterNo, Dictionary<string, object> parameters, StringBuilder preSql)
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

        protected override string GetOrderItemSql(ISortItem sortItem)
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

        protected override IEnumerable<Customer> List(string sql, Dictionary<string, object> parameters)
        {
            var customer = new CustomerProxy { CustomerId = "1", CustomerName = "name_1" };
            var history = new CustomerHistoryProxy { CustomerId = "1", HistoryNo = 1, Content = "xxx" };
            var detail = new CustomerHistoryDetail { CustomerId = "1", HistoryNo = 1, Text = "zzz" };

            customer.CustomerHistoryList.Add(history);
            history.CustomerHistoryDetailList.Add(detail);

            return new Customer[] { customer };
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
            CustomerProxy proxy = model as CustomerProxy;

            foreach (var history in proxy.DeletedHistoryList)
            {
            }
        }
    }

    public class CustomerApp
    {
        public CustomerApp()
        {
            _factory = new CustomerFactory();
            _repository = new CustomerRepository(_domainEvnetPublisher);
        }

        private IDomainEvnetPublisher _domainEvnetPublisher = null;
        private IFactory<Customer, CustomerCreationSpec> _factory;
        private IRepository<Customer, CustomerSelectItem, CustomerSortItem> _repository;

        public void Create(string customerName)
        {
            CustomerCreationSpec spec = new CustomerCreationSpec();
            spec.CustomerName = customerName;

            Customer model = _factory.Create(spec);

            CustomerHistory hisotry = model.NewHistory();

            hisotry.Content = "new";

            CustomerHistoryDetail detail = hisotry.NewDetail();
            detail.Text = "new";

            hisotry.AddDetail(detail);            

            model.AddHistory(hisotry);
            

            string[] errors;
            bool validate = model.Validate(out errors);

            if (!validate)
            {
                throw new Exception(errors[0]);
            }

            _repository.Save(model);
        }

        public void Update(string customerId, string customerName)
        {
            SelectionSpec<CustomerSelectItem, CustomerSortItem> spec
                = new SelectionSpec<CustomerSelectItem, CustomerSortItem>(new CustomerSelectItem.CustomerId(customerId));

            Customer model = _repository.Single(spec);

            model.CustomerName = customerName;

            CustomerHistory history = model.CustomerHistoryList.FirstOrDefault();

            if (history != null)
            {
                model.RemoveHistory(history);
            }

            string[] errors;
            bool validate = model.Validate(out errors);

            if (!validate)
            {
                throw new Exception(errors[0]);
            }

            _repository.Save(model);
        }

        public void Delete(string customerId)
        {
            SelectionSpec<CustomerSelectItem, CustomerSortItem> spec
                = new SelectionSpec<CustomerSelectItem, CustomerSortItem>(new CustomerSelectItem.CustomerId(customerId));

            Customer model = _repository.Single(spec);

            model.Delete();

            _repository.Save(model);
        }

        public object[] List(string[] customerIds, string likeCustomerName)
        {
            SelectionSpec<CustomerSelectItem, CustomerSortItem> spec
                = new SelectionSpec<CustomerSelectItem, CustomerSortItem>(new CustomerSelectItem.CustomerId(customerIds));

            spec.And(new CustomerSelectItem.LikeCustomerName(likeCustomerName));
            spec.Sort(new CustomerSortItem.CustomerId());

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
