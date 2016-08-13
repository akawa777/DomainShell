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

            _creditCardService = new CreditCardService();            
        }

        private Session _session;

        private CartReader _cartReader;
        private CartRepository _cartRepository;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;        
        private PaymentRepository _paymentRepository;

        private CreditCardService _creditCardService;  

        public CartItemData[] Get(string customerId)
        {
            List<CartItemReadObejct> cartList = _cartReader.GetItemList(customerId);

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
            using (Transaction tran = _session.BegingTran())
            {   
                List<CartItemModel> itemList = new List<CartItemModel>();
                foreach (CartItemData detail in items)
                {
                    CartItemModel item = new CartItemModel();
                    
                    item.Number = detail.Number;
                    item.Product = _productRepository.Get(detail.ProductId);

                    itemList.Add(item);
                }

                CartModel cart = new CartModel();

                cart.Create(customerId, itemList);

                _cartRepository.Save(cart);

                tran.Commit();
            }
        }

        public void Update(CartItemData[] items)
        {
            using (Transaction tran = _session.BegingTran())
            {
                CartModel cart = _cartRepository.Get(items[0].CartId);

                List<CartItemModel> itemList = new List<CartItemModel>();
                foreach (CartItemData item in items)
                {
                    CartItemModel itemModel = new CartItemModel();

                    itemModel.Number = item.Number;
                    itemModel.Product = _productRepository.Get(item.ProductId);

                    itemList.Add(itemModel);
                }

                cart.Update(itemList);

                _cartRepository.Save(cart);

                tran.Commit();
            }
        }

        public void Delete(string cartId)
        {            
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
                decimal postage = _cartReader.GetPostage(data.ShippingAddress);

                PaymentModel payment = cart.Checkout(data.ShippingAddress, postage);

                payment.Pay(
                        data.CreditCardNo,
                        data.CreditCardHolder,
                        data.CreditCardExpirationDate,
                        _creditCardService);

                _cartRepository.Save(cart);
                _paymentRepository.Save(payment);

                tran.Commit();
            }
        }
    }
}
