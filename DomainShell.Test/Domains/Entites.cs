using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainShell.Test.Domains.Entites
{
    public class LoginUser
    {
        public virtual string UserId { get; private set; }

        public virtual string UserName { get; set; }

        public virtual int RecordVersion { get; private set; }
    }

    public class OrderForm
    {
        public virtual int OrderId { get; set; }

        public virtual string UserId { get; set; }

        public virtual string OrderDate { get; set; }

        public virtual string ProductName { get; set; }

        public virtual decimal Price { get; set; }

        public virtual string CreditCardCode { get; set; }

        public virtual string PayId { get; private set; }        

        public virtual int RecordVersion { get; set; }
    }

    public class OrderFormCanceled
    {
        public virtual int OrderId { get; set; }

        public virtual string UserId { get; set; }

        public virtual string OrderDate { get; set; }

        public virtual string ProductName { get; set; }

        public virtual decimal Price { get; set; }

        public virtual string CreditCardCode { get; set; }

        public virtual string PayId { get; set; }        

        public virtual int RecordVersion { get; set; }
    }

    public class MonthlyOrderBudget
    {
        public string UserId { get; private set; }
        public decimal Budget { get; private set; }
    }
}