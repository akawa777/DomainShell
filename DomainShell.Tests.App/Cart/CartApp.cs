using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Domain.Cart;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.Domain.Product;
using DomainShell.Tests.Domain.Payment;
using DomainShell.Tests.Infrastructure;
using DomainShell.Tests.Infrastructure.Cart;
using DomainShell.Tests.Infrastructure.Customer;
using DomainShell.Tests.Infrastructure.Product;
using DomainShell.Tests.Infrastructure.Payment;

namespace DomainShell.Tests.Apps.Cart
{
    public class CartItemData
    {
        public string CartId { get; set; }
        public string CartItemId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Number { get; set; }
    }

    public class PaymentData
    {
        public string CartId { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
        public string ShippingAddress { get; set; }       
    }

    public class CheckoutData
    {
        public string CartId { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Postage { get; set; }
        public decimal PaymentAmount { get; set; }
    }

    public class CartApp
    {
        public CartApp()
        {
            _session = new Session();

            _cartReader = new CartReader(_session);
            _cartRepository = new CartRepository(_session);
            _customerRepository = new CustomerRepository(_session);
            _productRepository = new ProductRepository(_session);            
            _paymentRepository = new PaymentRepository(_session);                    

            _paymentService = new PaymentService();
            _postageService = new PostageService();
        }

        private Session _session;

        private CartReader _cartReader;
        private CartRepository _cartRepository;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;        
        private PaymentRepository _paymentRepository;

        private IPaymentService _paymentService;
        private IPostageService _postageService;  

        public CartItemData[] Get(string customerId)
        {
            List<CartItemReadModel> cartList = _cartReader.GetItemList(customerId);

            return cartList.Select(x => new CartItemData
            {
                CartId = x.CartId,
                CartItemId = x.CartItemId,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Price = x.Price,
                Number = x.Number
            }).ToArray();
        }

        public void Create(string customerId, CartItemData[] items)
        {
            using (_session.Open())
            using (Transaction tran = _session.BegingTran())
            {
                CartModel cart = new CartModel();
                cart.Customer = _customerRepository.Get(customerId);

                cart.CartItems = new List<CartItemModel>();

                foreach (CartItemData detail in items)
                {
                    CartItemModel item = new CartItemModel();
                    
                    item.Number = detail.Number;
                    item.Product = _productRepository.Get(detail.ProductId);

                    cart.CartItems.Add(item);
                }

                cart.Create();

                _cartRepository.Save(cart);

                tran.Commit();
            }
        }

        public void Update(CartItemData[] items)
        {
            using (_session.Open())
            using (Transaction tran = _session.BegingTran())
            {
                CartModel cart = _cartRepository.Get(items[0].CartId);                

                cart.CartItems = new List<CartItemModel>();

                foreach (CartItemData item in items)
                {
                    CartItemModel itemModel = new CartItemModel();

                    itemModel.Number = item.Number;
                    itemModel.Product = _productRepository.Get(item.ProductId);

                    cart.CartItems.Add(itemModel);
                }

                cart.Update();

                _cartRepository.Save(cart);

                tran.Commit();
            }
        }

        public void Delete(string cartId)
        {
            using (_session.Open())
            using (Transaction tran = _session.BegingTran())
            {
                CartModel cart = _cartRepository.Get(cartId);
                cart.Delete();

                _cartRepository.Save(cart);

                tran.Commit();
            }
        }

        public void Pay(PaymentData data)
        {
            using (Transaction tran = _session.BegingTran())
            {
                CartModel cart = _cartRepository.Get(data.CartId);

                PaymentModel payment = cart.Checkout(data.ShippingAddress, _postageService);

                payment.Pay(
                        data.CreditCardNo,
                        data.CreditCardHolder,
                        data.CreditCardExpirationDate,
                        _paymentService);

                _cartRepository.Save(cart);
                _paymentRepository.Save(payment);

                tran.Commit();
            }
        }
    }
}
