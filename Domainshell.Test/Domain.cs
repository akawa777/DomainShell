using System;
using System.Linq;
using System.Collections.Generic;
using Domainshell;

namespace Domainshell.Test
{
    public class OrderModel : IDomainEventAuthor
    {
        public static OrderModel NewOrder()
        {
            OrderModel orderModel = new OrderModel();            
            orderModel.OrderId = Guid.NewGuid().ToString();
            orderModel.Dirty = true;
            return orderModel;
        }

        public string OrderId { get; private set; }       

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string PayId { get; private set; }

        public int RecordVersion { get; private set; }

        public bool Dirty { get; private set; }

        public bool Deleted { get; private set; }

        private List<IDomainEvent> _events = new List<IDomainEvent>();
        
        public IEnumerable<IDomainEvent> GetEvents()
        {
            return _events;
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        public void Regist(IOrderValidator orderValidator)
        {
            orderValidator.ValidateWhenRegist(this);

            Dirty = true;
        }

        public void Complete(IOrderValidator orderValidator, ICreditCardService creditCardService, string creditCardCode)
        {
            orderValidator.ValidateWhenComplete(this);

            Dirty = true;
            creditCardService.Pay(creditCardCode, Price, out string payId);
            PayId = payId;

            _events.Add(new OrderCompletedEvent{ Async = false, OrderId = OrderId });
            _events.Add(new OrderCompletedExceptionEvent{ OrderId = OrderId });
        }

        public void SendCompletedMail(IMailService mailService)
        {
            mailService.Send("x", "x", "x");
        }

        public void CancelCompleted(ICreditCardService creditCardService)
        {
            Dirty = true;

            if (!string.IsNullOrEmpty(PayId))
            {
                creditCardService.Cancel(PayId);
                PayId = null;
            }
        }
    }

    public class OrderCompletedEvent : IDomainOuterTranEvent
    {   
        public bool Async { get; set; }
        public string OrderId { get; set; }
    }

    public class OrderCompletedExceptionEvent : IDomainExceptionEvent
    {   
        public Exception Exception { get; set; }
        public string OrderId { get; set; }
    }

    public interface IOrderRepository
    {
        OrderModel Find(string orderId, bool throwError = false);
        void Commit(OrderModel orderModel);
    }

    public interface IOrderValidator
    {
        void ValidateWhenRegist(OrderModel orderModel);
        void ValidateWhenComplete(OrderModel orderModel);
    }

    public interface ICreditCardService
    {
        void Pay(string creditCardCord, decimal price, out string payId);
        void Cancel(string payId);
    }

    public interface IMailService
    {
        void Send(string emailAddress, string title, string contents);
    }

    public class OrderValidator : IOrderValidator
    {
        public void ValidateWhenRegist(OrderModel orderModel)
        {

        }

        public void ValidateWhenComplete(OrderModel orderModel)
        {

        }
    }

    public class CreditCardService : ICreditCardService
    {
        public void Pay(string creditCardCord, decimal price, out string payId)
        {
            payId = Guid.NewGuid().ToString();
            Console.WriteLine($"{nameof(CreditCardService)} {nameof(Pay)}");
        }

        public void Cancel(string payId)
        {
            Console.WriteLine($"{nameof(CreditCardService)} {nameof(Cancel)}");
        }
    }

    public class MailService : IMailService
    {
        public void Send(string emailAddress, string title, string contents)
        {
            Console.WriteLine($"{nameof(MailService)} {nameof(Send)}");
        }
    }

    public class OrderCompletedEventHandler : IDomainEventHandler<OrderCompletedEvent>, IDomainEventHandler<OrderCompletedExceptionEvent>
    {
        public OrderCompletedEventHandler(ISession session, IOrderRepository orderRepository, ICreditCardService creditCardService, IMailService mailService)
        {
            _session = session;
            _orderRepository = orderRepository;
            _creditCardService = creditCardService;
            _mailService = mailService;
        }

        private ISession _session;
        private IOrderRepository _orderRepository;      
        private ICreditCardService  _creditCardService;
        private IMailService _mailService;

        public void Handle(OrderCompletedEvent domainEvent)
        {
            try
            {
                using(_session.Open())
                {
                    OrderModel orderModel = _orderRepository.Find(domainEvent.OrderId);

                    orderModel.SendCompletedMail(_mailService);
                }
            }
            catch(Exception e)
            {
                _session.OnException(e);
                throw e;
            }
        }

        public void Handle(OrderCompletedExceptionEvent domainEvent)
        {
            try
            {
                using(var tran = _session.Tran())
                {
                    OrderModel orderModel = _orderRepository.Find(domainEvent.OrderId);

                    orderModel.CancelCompleted(_creditCardService);

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
    }
}