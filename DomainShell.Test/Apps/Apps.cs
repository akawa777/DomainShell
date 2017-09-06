using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using DomainShell.Test.Domains;

namespace DomainShell.Test.Apps
{
    public class OrderCommandApp
    {
        public OrderCommandApp(
            IOrderRepository orderRepository, 
            IWriteRepository<OrderModel> writeRepository, 
            IOrderValidator orderValidator, 
            ICreditCardService creditCardService)
        {            
            _orderRepository = orderRepository;
            _writeRepository = writeRepository;
            _orderValidator = orderValidator;
            _creditCardService = creditCardService;
        }

        private IOrderRepository _orderRepository;
        private IWriteRepository<OrderModel> _writeRepository;
        private IOrderValidator _orderValidator;
        private ICreditCardService _creditCardService;
        private IOrderSummaryReader _orderSummaryReader;

        public void Register(OrderDto orderDto)
        {
            try
            {
                using(var tran = Session.Tran())
                {
                    OrderModel orderModel;

                    if (orderDto.OrderId < 1) orderModel = OrderModel.NewOrder();
                    else orderModel = _orderRepository.Find(orderDto.OrderId, true);

                    Map(orderDto, orderModel);

                    orderModel.Register(_orderValidator);

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

                    _writeRepository.Save(orderModel);

                    tran.Complete();
                }
            }
            catch(Exception e)
            {
                Session.OnException(e);
                throw e;
            }
        }

        public void Cancel(OrderDto orderDto, string reason)
        {
            try
            {
                using (var tran = Session.Tran())
                {
                    OrderModel orderModel = _orderRepository.Find(orderDto.OrderId, true);
                    Map(orderDto, orderModel);

                    orderModel.Cancel(_orderValidator);

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
            model.ProductName = dto.ProductName;         
            model.Price = dto.Price;
            model.LastUserId = dto.LastUserId;
        }
    }

    public class OrderQueryApp
    {
        public OrderQueryApp(IOrderRepository orderRepository, IOrderCanceledRepository orderCanceledRepository, IOrderSummaryReader orderSummaryReader)
        {            
            _orderRepository = orderRepository;
            _orderCanceledRepository = orderCanceledRepository;
            _orderSummaryReader = orderSummaryReader;
        }
        
        private IOrderRepository _orderRepository;
        private IOrderCanceledRepository _orderCanceledRepository;
        private IOrderSummaryReader _orderSummaryReader;

        public OrderDto Find(int orderId)
        {
            try
            {
                using(Session.Open())
                {
                    OrderModel orderModel = _orderRepository.Find(orderId);

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

        public OrderSummaryDto[] GetOrderSummary()
        {
            try
            {
                using (Session.Open())
                {
                    IEnumerable<OrderSummaryValue> orderSummaryValues = _orderSummaryReader.GetSummary();

                    return orderSummaryValues.Select(Map).ToArray();
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

        private OrderDto Map(OrderCanceledModel model)
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

        private OrderSummaryDto Map(OrderSummaryValue value)
        {
            if (value == null) return null;

            OrderSummaryDto dto = new OrderSummaryDto();            
            dto.ProductName = value.ProductName;
            dto.TotalPrice = value.TotalPrice;
            dto.TotalOrderNo= value.TotalOrderNo;

            return dto;
        }
    }

    public class OrderDto
    {
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string PayId { get; set; }
        public string LastUserId { get; set; }
        public int RecordVersion { get; set; }        
    }

    public class OrderSummaryDto
    {
        public string ProductName { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalOrderNo { get; set; }
    }
}
