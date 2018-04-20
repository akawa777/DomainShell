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
            IOrderService orderService,            
            IUserRepository userRepository)
        {            
            _orderRepository = orderRepository;
            _orderService = orderService;            
            _userRepository = userRepository;
        }

        private IOrderRepository _orderRepository;
        private IOrderService _orderService;        
        private IUserRepository _userRepository;

        public void Register(OrderDto orderDto)
        {
            if (orderDto == null) throw new Exception("orderDto is required.");

            try
            {
                using(var tran = Session.Tran())
                {
                    Order order;

                    if (orderDto.OrderId < 1) order = Order.NewOrder();
                    else order = _orderRepository.Find(orderDto.OrderId, true);

                    Map(orderDto, order);

                    order.Register(_orderService);                    

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

        public void Complete(OrderDto orderDto, string creditCardCode)
        {
            if (orderDto == null) throw new Exception("orderDto is required.");

            try
            {
                using(var tran = Session.Tran())
                {
                    Order order = _orderRepository.Find(orderDto.OrderId, true);
                    Map(orderDto, order);

                    order.Complete(_orderService, creditCardCode);

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

        public void Cancel(OrderDto orderDto)
        {
            if (orderDto == null) throw new Exception("orderDto is required.");

            try
            {
                using (var tran = Session.Tran())
                {
                    Order order = _orderRepository.Find(orderDto.OrderId, true);
                    Map(orderDto, order);

                    order.Cancel();

                    _orderRepository.Save(order);

                    tran.Complete();
                }
            }
            catch (Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        private void Map(OrderDto dto, Order model)
        {
            model.User = UserValue.Create(_userRepository.Find(dto.UserId, true));
            model.OrderDate = DateTime.ParseExact(dto.OrderDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            model.ProductName = dto.ProductName;         
            model.Price = dto.Price;            
        }
    }   
}
