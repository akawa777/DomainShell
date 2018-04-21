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
        public OrderQueryApp(
            IOrderRepository orderRepository,
            IOrderReadRepository orderReadRepository)
        {            
            _orderRepository = orderRepository;            
            _orderReadRepository = orderReadRepository;
        }
        
        private IOrderRepository _orderRepository;
        private IOrderReadRepository _orderReadRepository;
        
        public OrderDto Find(int orderId)
        {
            try
            {
                using(Session.Open())
                {
                    var order = _orderRepository.Find(orderId);

                    return Map(order);
                }
            }
            catch(Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        public OrderReadDto GetLastByUser(string userId)
        {
            try
            {
                using (Session.Open())
                {
                    var orderRead = _orderReadRepository.GetLastByUser(userId);

                    return Map(orderRead);
                }
            }
            catch (Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        public System.IO.Stream IssueCertificate(int orderId)
        {
            try
            {
                using (Session.Open())
                {
                    var order = _orderRepository.Find(orderId);
                    var certificate = order.IssueCertificate();

                    return certificate;
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

            var dto = new OrderDto
            {
                OrderId = model.OrderId,
                UserId = model.UserId,
                OrderDate = model.OrderDate.Value.ToString("yyyyMMdd"),
                ProductName = model.ProductName,
                Price = model.Price,
                CreditCardCode = model.CreditCardCode,
                PaymentId = model.PaymentId
            };

            return dto;
        }

        private OrderReadDto Map(OrderRead model)
        {
            if (model == null) return null;

            var dto = new OrderReadDto
            {
                OrderId = model.OrderId,                
                ProductName = model.ProductName,
                Price = model.Price
            };

            return dto;
        }
    }
}
