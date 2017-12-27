using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using DomainShell.Test.Domains;
using DomainShell.Test.Domains.Order;

namespace DomainShell.Test.Apps
{   
    public class OrderQueryApp
    {
        public OrderQueryApp(IOrderRepository orderRepository, IOrderCanceledRepository orderCanceledRepository, IMonthlyOrderRepository monthlyOrderRepository)
        {            
            _orderRepository = orderRepository;
            _orderCanceledRepository = orderCanceledRepository;
             _monthlyOrderRepository = monthlyOrderRepository;
        }
        
        private IOrderRepository _orderRepository;
        private IOrderCanceledRepository _orderCanceledRepository;
        private IMonthlyOrderRepository _monthlyOrderRepository;

        public object[] GetMonthlyOrderBudgets()
        {
            try
            {
                using(Session.Open())
                {
                    return _monthlyOrderRepository.GetMonthlyOrderBudgets();
                }
            }
            catch(Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        public OrderDto Find(int orderId)
        {
            try
            {
                using(Session.Open())
                {
                    OrderModel orderModel = _orderRepository.Find(orderId);

                    return Map(orderModel);
                }
            }
            catch(Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        public OrderDto GetLastByUser(string userId)
        {
            try
            {
                using (Session.Open())
                {
                    OrderModel orderModel = _orderRepository.GetLastByUser(userId);

                    return Map(orderModel);
                }
            }
            catch (Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        public OrderDto GetCanceledByOrderId(int orderId)
        {
            try
            {
                using (Session.Open())
                {
                    OrderCanceledModel orderCanceledModel = _orderCanceledRepository.Find(orderId);

                    return Map(orderCanceledModel);
                }
            }
            catch (Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        private OrderDto Map(OrderModel model)
        {
            if (model == null) return null;

            OrderDto dto = new OrderDto();
            dto.OrderId = model.OrderId;
            dto.UserId = model.User.UserId;
            dto.OrderDate = model.OrderDate.Value.ToString("yyyyMMdd");
            dto.ProductName = model.ProductName;
            dto.Price = model.Price;
            dto.CreditCardCode = model.CreditCardCode;
            dto.PayId = model.PayId;            
            dto.RecordVersion = model.RecordVersion;

            return dto;
        }

        private OrderDto Map(OrderCanceledModel model)
        {
            if (model == null) return null;

            OrderDto dto = new OrderDto();
            dto.OrderId = model.OrderId;
            dto.ProductName = model.ProductName;
            dto.Price = model.Price;
            dto.CreditCardCode = model.CreditCardCode;
            dto.PayId = model.PayId;
            dto.UserId = model.User.UserId;
            dto.RecordVersion = model.RecordVersion;

            return dto;
        }
    }
}
