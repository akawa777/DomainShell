using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Domain.Cart;
using DomainShell.Tests.Domain.Payment;
using DomainShell.Tests.Domain.Delivery;
using DomainShell.Tests.Infrastructure;
using DomainShell.Tests.Infrastructure.Cart;
using DomainShell.Tests.Infrastructure.Payment;
using DomainShell.Tests.Infrastructure.Delivery;

namespace DomainShell.Tests.Apps.Payment
{
    public class PaymentData
    {
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Postage { get; set; }

        public string CartId { get; set; }
    }

    public class PaymentApp
    {
        public PaymentApp()
        {
            _session = new Session();

            _cartRepository = new CartRepository(_session);
            _paymentRepository = new PaymentRepository(_session);
            _paymentService = new PaymentService();
            _deliveryRepository = new DeliveryRepository(_session);
        }

        private Session _session;
        private CartRepository _cartRepository;
        private PaymentRepository _paymentRepository;
        private PaymentService _paymentService;
        private DeliveryRepository _deliveryRepository;        

        private bool Validate(PaymentData data)
        {
            return true;
        }
        
        public bool Pay(PaymentData data)
        {
            if (!Validate(data))
            {
                return false;
            }

            using (_session.Open())
            {
                CartModel cart = _cartRepository.Get(data.CartId);

                PaymentModel payment = new PaymentModel();

                payment.Add(cart);

                payment.Pay(
                        data.CreditCardNo,
                        data.CreditCardHolder,
                        data.CreditCardExpirationDate,
                        data.ShippingAddress,
                        data.Postage,
                        _paymentService);

                using (Transaction tran = _session.BegingTran())
                {
                    _paymentRepository.Save(payment);
                    _cartRepository.Save(cart);

                    tran.Commit();
                }

                DeliveryModel delivery = new DeliveryModel();

                delivery.Send(payment);

                using (Transaction tran = _session.BegingTran())
                {
                    _deliveryRepository.Save(delivery);
                    tran.Commit();
                }
            }

            return true;
        }
    }
}
