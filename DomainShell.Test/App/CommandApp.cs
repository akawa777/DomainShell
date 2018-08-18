using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using DomainShell.Test.Domain.OrderAggregate;

namespace DomainShell.Test.App
{
    public class OrderCommandApp
    {
        public OrderCommandApp(
            IOrderRepository orderRepository,
            IOrderService orderService)
        {            
            _orderRepository = orderRepository;
            _orderService = orderService;
        }

        private IOrderRepository _orderRepository;
        private IOrderService _orderService;

        public void Pay(OrderDto orderDto, string creditCardCode)
        {            
            if (orderDto == null) throw new Exception("orderDto is required.");

            using (var tran = Session.Tran())
            {
                Order order = Order.Create(orderDto.SpecialOrderFlg);

                Map(orderDto, order);

                order.Pay(_orderService, creditCardCode);

                _orderRepository.Save(order);

                tran.Complete();
            }
        }

        private void Map(OrderDto dto, Order model)
        {
            model.UserId = dto.UserId;
            model.OrderDate = DateTime.ParseExact(dto.OrderDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            model.ProductName = dto.ProductName;         
            model.Price = dto.Price;                
        }
    }   
}
