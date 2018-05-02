using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using DomainShell;
using DomainShell.Test.Domain.OrderAggregate;

namespace DomainShell.Test.App
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

        public OrderDto GetLastByUser(string userId)
        {
            try
            {
                using (Session.Open())
                {
                    var order = _orderRepository.GetLastByUser(userId);

                    return Map(order);
                }
            }
            catch (Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        public Stream IssueCertificate(int orderId)
        {
            try
            {
                using (Session.Open())
                {
                    var orderRead = _orderReadRepository.Find(orderId);

                    if (orderRead == null) throw new Exception("order not found.");

                    var certificate = orderRead.IssueCertificate();

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
    }
}
