using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using DomainShell.Test.Domains;
using DomainShell.Test.Domains.OrderDomain;

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
                    Order order = _orderRepository.Find(orderId);

                    return Map(order);
                }
            }
            catch(Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        public OrderDto FindWithSendMail(int orderId)
        {
            try
            {
                using (Session.Open())
                {
                    Order order = _orderRepository.Find(orderId);
                    order.SendCompleteMail();

                    return Map(order);
                }
            }
            catch (Exception e)
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
                    Order order = _orderRepository.GetLastByUser(userId);

                    return Map(order);
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

        private OrderDto Map(Order model)
        {
            if (model == null) return null;

            OrderDto dto = new OrderDto
            {
                OrderId = model.OrderId,
                UserId = model.User.UserId,
                OrderDate = model.OrderDate.Value.ToString("yyyyMMdd"),
                ProductName = model.ProductName,
                Price = model.Price,
                CreditCardCode = model.CreditCardCode,
                PayId = model.PayId,
                RecordVersion = model.RecordVersion
            };

            return dto;
        }

        private OrderDto Map(OrderCanceledModel model)
        {
            if (model == null) return null;

            OrderDto dto = new OrderDto
            {
                OrderId = model.OrderId,
                ProductName = model.ProductName,
                Price = model.Price,
                CreditCardCode = model.CreditCardCode,
                PayId = model.PayId,
                UserId = model.User.UserId,
                RecordVersion = model.RecordVersion
            };

            return dto;
        }
    }
}
