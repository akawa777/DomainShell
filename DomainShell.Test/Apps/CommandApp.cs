using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using DomainShell.Test.Domains;
using DomainShell.Test.Domains.UserDomain;
using DomainShell.Test.Domains.OrderDomain;

namespace DomainShell.Test.Apps
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

        public void Pay(OrderDto orderDto, string creditCardCode, bool isSpecialOrder)
        {
            if (orderDto == null) throw new Exception("orderDto is required.");

            try
            {
                using(var tran = Session.Tran())
                {
                    Order order = Order.Create(isSpecialOrder);

                    Map(orderDto, order);

                    order.Pay(_orderService, creditCardCode);

                    _orderRepository.Save(order);

                    tran.Complete();
                }
            }
            catch(Exception e)
            {
                Session.OnException(e);
                throw e;
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
