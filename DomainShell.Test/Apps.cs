using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test
{
    public class OrderCommandApp
    {
        public OrderCommandApp(IOrderRepository orderRepository, IOrderValidator orderValidator, ICreditCardService creditCardService)
        {            
            _orderRepository = orderRepository;
            _orderValidator = orderValidator;
            _creditCardService = creditCardService;
        }

        private IOrderRepository _orderRepository;      
        private IOrderValidator _orderValidator;
        private ICreditCardService _creditCardService;

        public void Regist(OrderDto orderDto)
        {
            try
            {
                using(var tran = Session.Tran())
                {
                    OrderModel orderModel;

                    if (string.IsNullOrEmpty(orderDto.OrderId)) orderModel = OrderModel.NewOrder();
                    else orderModel = _orderRepository.Find(orderDto.OrderId, true);

                    Map(orderDto, orderModel);

                    orderModel.Regist(_orderValidator);

                    _orderRepository.Apply(orderModel);

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
            try
            {
                using(var tran = Session.Tran())
                {
                    OrderModel orderModel = _orderRepository.Find(orderDto.OrderId, true);
                    Map(orderDto, orderModel);

                    orderModel.Complete(_orderValidator, _creditCardService, creditCardCode);

                    _orderRepository.Apply(orderModel);

                    tran.Complete();
                }
            }
            catch(Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        private void Map(OrderDto dto, OrderModel model)
        {
            model.ProductName = dto.ProductName;         
            model.Price = dto.Price;
            model.LastUserId = dto.LastUserId;
        }
    }

    public class OrderQueryApp
    {
        public OrderQueryApp(IOrderRepository orderRepository)
        {            
            _orderRepository = orderRepository;
        }
        
        private IOrderRepository _orderRepository;           

        public OrderDto Find(string orderId)
        {
            try
            {
                using(Session.Open())
                {
                    OrderModel orderModel = _orderRepository.Find(orderId, true);

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

        private OrderDto Map(OrderModel model)
        {
            if (model == null) return null;

            OrderDto dto = new OrderDto();
            dto.OrderId = model.OrderId;
            dto.ProductName = model.ProductName;
            dto.Price = model.Price;
            dto.PayId = model.PayId;
            dto.LastUserId = model.LastUserId;
            dto.RecordVersion = model.RecordVersion;

            return dto;
        }
    }

    public class OrderDto
    {
        public string OrderId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string PayId { get; set; }
        public string LastUserId { get; set; }
        public int RecordVersion { get; set; }        
    }
}
