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
    public class Product
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }   

    public class CartItem
    {
        public string CartId { get; set; }
        public string CartItemId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Number { get; set; }
    }

    public class CartAddItem
    {
        public string CustomerId { get; set; }        
        public string ProductId { get; set; }             
        public int Number { get; set; }
    }

    public class CartAddItemResult
    {
        public bool Success { get; set; }
        public string CartItemId { get; set; }
    }

    public class CartRemoveItem
    {
        public string CustomerId { get; set; }
        public string CartItemId { get; set; }
    }

    public class CartRemoveItemResult
    {
        public bool Success { get; set; }
    }

    public class PaymentAmountQuery
    {
        public string CustomerId { get; set; }
        public string ShippingAddress { get;set; }
    }   

    public class Payment
    {
        public string CartId { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
        public string ShippingAddress { get; set; }       
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
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

        public Product[] GetProducts()
        {
            using (_session.Open())
            {
                List<ProductModel> productList = _productRepository.GetAll();

                return productList
                    .Select(x => new Product { ProductId = x.ProductId, ProductName = x.ProductName, Price = x.Price })
                    .ToArray();
            }
        }

        public CartItem[] Get(string customerId)
        {
            using (_session.Open())
            {
                List<CartItemReadObject> readObjects = _cartReader.GetItemList(customerId);

                return readObjects.Select(x => new CartItem
                {
                    CartId = x.CartId,
                    CartItemId = x.CartItemId,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    Number = x.Number
                }).ToArray();
            }
        }

        public CartAddItemResult Add(CartAddItem item)
        {
            using (Transaction tran = _session.BegingTran())
            {
                CartModel cartModel = _cartRepository.Get(item.CustomerId);

                if (cartModel == null)
                {
                    cartModel = new CartModel();
                    cartModel.Create(item.CustomerId);
                }

                CartItemModel itemModel = new CartItemModel();

                itemModel.Number = item.Number;
                itemModel.ProductId = item.ProductId;
                itemModel.Product = _productRepository.Find(item.ProductId);

                cartModel.AddItem(itemModel);

                _cartRepository.Save(cartModel);

                tran.Commit();

                return new CartAddItemResult
                {
                    Success = true,
                    CartItemId = itemModel.CartItemId
                };
            }            
        }

        public CartRemoveItemResult Remove(CartRemoveItem item)
        {            
            using (Transaction tran = _session.BegingTran())
            {
                CartModel cartModel = _cartRepository.Get(item.CustomerId);
                cartModel.RemoveItem(item.CartItemId);

                _cartRepository.Save(cartModel);

                tran.Commit();

                return new CartRemoveItemResult
                {
                    Success = true
                };
            }
        }

        public decimal GetPaymentAmount(PaymentAmountQuery query)
        {
            using (_session.Open())
            {
                CartModel cartModel = _cartRepository.Get(query.CustomerId);
                decimal postage = _cartReader.GetPostage(query.ShippingAddress);

                PaymentModel paymentModel = cartModel.Checkout(query.ShippingAddress, postage);

                return paymentModel.PaymentAmount();
            }
        }

        public PaymentResult Pay(Payment payment)
        {
            using (Transaction tran = _session.BegingTran())
            {
                CartModel cartModel = _cartRepository.Get(payment.CartId);
                decimal postage = _cartReader.GetPostage(payment.ShippingAddress);

                PaymentModel paymentModel = cartModel.Checkout(payment.ShippingAddress, postage);

                paymentModel.Pay(
                        payment.CreditCardNo,
                        payment.CreditCardHolder,
                        payment.CreditCardExpirationDate,
                        _creditCardService);

                _cartRepository.Save(cartModel);
                _paymentRepository.Save(paymentModel);

                tran.Commit();

                return new PaymentResult
                {
                    Success = true
                };
            }
        }
    }
}
