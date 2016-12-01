using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Commerce.Domain
{
    public class PurchaseDetailId : IValue
    {
        public PurchaseDetailId(int purchaseId, int purchaseDetailNo)
        {
            PurchaseId = purchaseId;
            PurchaseDetailNo = purchaseDetailNo;
        }

        public int PurchaseId
        {
            get;
            protected set;
        }

        public int PurchaseDetailNo
        {
            get;
            protected set;
        }

        public string Value
        {
	        get 
            {
                return string.Join(":", PurchaseId, PurchaseDetailNo);
            }
        }
    }

    public class PurchaseDetailEntity : IEntity<PurchaseDetailId>
    {
        public PurchaseDetailId Id
        {
            get;
            protected set;
        }

        public int ProductId
        {
            get;
            set;
        }

        public string ProductName
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }

        public decimal Price
        {
            get;
            set;
        }
    }

    public class PurchaseEntity : IAggregateRoot<int>
    {
        public PurchaseEntity(int id)
        {

        }

        public IEnumerable<IDomainEvent> GetEvents()
        {
            throw new NotImplementedException();
        }

        public void ClearEvents()
        {
            throw new NotImplementedException();
        }

        public int Id
        {
            get;
            protected set;
        }

        public int CustomerId
        {
            get;
            set;
        }

        public string PurchaseDate
        {
            get;
            set;
        }

        public CreditCardValue CreditCard
        {
            get;
            set;
        }

        public IReadOnlyList<PurchaseDetailEntity> PurchaseDetailList
        {
            get;
            protected set;
        }

        public virtual void Validate(IValidationSpec<PurchaseEntity> spec)
        {
            string[] errors;
            if (!spec.Validate(this, out errors))
            {
                throw new Exception(errors[0]);
            }
        }

        public virtual void Delete()
        {
            throw new Exception("Purchase can not delete");
        }
    }
}
