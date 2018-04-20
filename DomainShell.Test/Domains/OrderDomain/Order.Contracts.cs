using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;

namespace DomainShell.Test.Domains.OrderDomain
{    
    public interface IOrderRepository : IWriteRepository<Order>
    {
        Order Find(int orderId, bool throwError = false);        
        Order GetLastByUser(string userId);
    }

    public interface IOrderCanceledRepository : IWriteRepository<OrderCanceledModel> 
    {
        OrderCanceledModel Find(int orderId, bool throwError = false);
    }

    public interface IMonthlyOrderRepository
    {        
        MonthlyOrder GetMonthlyByUserId(string userId, DateTime orderDate, int excludeOrderId = 0);        
        object[] GetMonthlyOrderBudgets();
    }

    public interface IOrderService
    {
        string Pay(Order Order);
        void Cancel(Order Order);
        void SendMail(Order Order);
        bool IsOverBudget(Order Order);
    }
}