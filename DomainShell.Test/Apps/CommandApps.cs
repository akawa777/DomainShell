using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using DomainShell.Test.Domains;
using DomainShell.Test.Domains.User;
using DomainShell.Test.Domains.Order;

namespace DomainShell.Test.Apps
{
    public class OrderCommandApp
    {
        public OrderCommandApp(
            IOrderRepository orderRepository,
            IOrderBudgetCheckService orderBudgetCheckService,
            ICreditCardService creditCardService,
            IUserRepository userRepository)
        {            
            _orderRepository = orderRepository;
            _orderBudgetCheckService = orderBudgetCheckService;
            _creditCardService = creditCardService;
            _userRepository = userRepository;
        }

        private IOrderRepository _orderRepository;
        private IOrderBudgetCheckService _orderBudgetCheckService;
        private ICreditCardService _creditCardService;
        private IUserRepository _userRepository;

        public void Register(OrderDto orderDto)
        {
            if (orderDto == null) throw new Exception("orderDto is required.");

            try
            {
                using(var tran = Session.Tran())
                {
                    OrderModel orderModel;

                    if (orderDto.OrderId < 1) orderModel = OrderModel.NewOrder();
                    else orderModel = _orderRepository.Find(orderDto.OrderId, true);

                    Map(orderDto, orderModel);

                    orderModel.Register(_orderBudgetCheckService);                    

                    _orderRepository.Save(orderModel);

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
                    OrderModel orderModel = _orderRepository.Find(orderDto.OrderId, true);
                    Map(orderDto, orderModel);

                    orderModel.Complete(_creditCardService, creditCardCode);

                    _orderRepository.Save(orderModel);

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
                    OrderModel orderModel = _orderRepository.Find(orderDto.OrderId, true);
                    Map(orderDto, orderModel);

                    orderModel.Cancel();

                    _orderRepository.Save(orderModel);

                    tran.Complete();
                }
            }
            catch (Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        private void Map(OrderDto dto, OrderModel model)
        {
            model.User = UserValue.Create(_userRepository.Find(dto.UserId, true));
            model.OrderDate = DateTime.ParseExact(dto.OrderDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            model.ProductName = dto.ProductName;         
            model.Price = dto.Price;            
        }
    }   
}
