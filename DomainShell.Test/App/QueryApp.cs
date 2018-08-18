using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using DomainShell.App;
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
            using (Session.Open())
            {
                var order = _orderRepository.Find(orderId);

                return Map(order);
            }
        }

        public OrderDto GetLastByUser(string userId)
        {
            using (Session.Open())
            {
                var order = _orderRepository.GetLastByUser(userId);

                return Map(order);
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
                PaymentId = model.PaymentId,
                SpecialOrderFlg = model is SpecialOrder

            };

            return dto;
        }
    }
}
