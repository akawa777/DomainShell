using System;
using System.Linq;
using System.Collections.Generic;
using Domainshell;

namespace Domainshell.Test
{
    public class OrderCommandApp
    {
        public OrderCommandApp(ISession session, IOrderRepository orderRepository, IOrderValidator orderValidator, ICreditCardService creditCardService)
        {
            _session = session;
            _orderRepository = orderRepository;
            _orderValidator = orderValidator;
            _creditCardService = creditCardService;
        }

        private ISession _session;
        private IOrderRepository _orderRepository;      
        private IOrderValidator _orderValidator;
        private ICreditCardService _creditCardService;

        public string Regist(OrderDto orderDto)
        {
            try
            {
                using(var tran = _session.Tran())
                {
                    OrderModel orderModel;

                    if (string.IsNullOrEmpty(orderDto.OrderId)) orderModel = OrderModel.NewOrder();
                    else orderModel = _orderRepository.Find(orderDto.OrderId, true);

                    Map(orderDto, orderModel);

                    orderModel.Regist(_orderValidator);

                    _orderRepository.Commit(orderModel);

                    tran.Complete();

                    return orderModel.OrderId;                    
                }
            }
            catch(Exception e)
            {
                _session.OnException(e);
                throw e;
            }
        }

        public void Complete(OrderDto orderDto, string creditCardCode)
        {
            try
            {
                using(var tran = _session.Tran())
                {
                    OrderModel orderModel = _orderRepository.Find(orderDto.OrderId, true);
                    Map(orderDto, orderModel);

                    orderModel.Complete(_orderValidator, _creditCardService, creditCardCode);

                    _orderRepository.Commit(orderModel);

                    tran.Complete();
                }
            }
            catch(Exception e)
            {
                _session.OnException(e);
                throw e;
            }
        }

        private void Map(OrderDto dto, OrderModel model)
        {
            model.ProductName = dto.ProductName;         
            model.Price = dto.Price;   
        }
    }

    public class OrderQueryApp
    {
        public OrderQueryApp(ISession session, IOrderRepository orderRepository)
        {
            _session = session;
            _orderRepository = orderRepository;
        }

        private ISession _session;
        private IOrderRepository _orderRepository;           

        public OrderDto Find(string orderId)
        {
            try
            {
                using(_session.Open())
                {
                    OrderModel orderModel = _orderRepository.Find(orderId, true);

                    return Map(orderModel);
                }
            }
            catch(Exception e)
            {
                _session.OnException(e);
                throw e;
            }
        }

        private OrderDto Map(OrderModel model)
        {
            OrderDto dto = new OrderDto();
            dto.OrderId = model.OrderId;
            dto.ProductName = model.ProductName;
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
        public int RecordVersion { get; set; }        
    }
}
