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
    public class Customer
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
    }   

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
        public CartAddItemResult()
        {
            Success = true;
            Messages = new List<string>();
        }

        public bool Success { get; set; }
        public List<string> Messages { get; set; }
        public string CartItemId { get; set; }
    }

    public class CartUpdateItem
    {
        public string CustomerId { get; set; }
        public string CartItemId { get; set; }
        public int Number { get; set; }
    }

    public class CartUpdateItemResult
    {
        public CartUpdateItemResult()
        {
            Success = true;
            Messages = new List<string>();
        }

        public List<string> Messages { get; set; }
        public bool Success { get; set; }        
    }

    public class CartRemoveItem
    {
        public string CustomerId { get; set; }
        public string CartItemId { get; set; }
    }

    public class CartRemoveItemResult
    {
        public CartRemoveItemResult()
        {
            Success = true;
            Messages = new List<string>();
        }

        public List<string> Messages { get; set; }
        public bool Success { get; set; }
    }

    public class PaymentAmountInfo
    {
        public decimal Postage { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PaymentAmount { get; set; }
    }

    public class Payment
    {
        public string CustomerId { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardHolder { get; set; }
        public string CreditCardExpirationDate { get; set; }
        public string ShippingAddress { get; set; }       
    }

    public class PaymentResult
    {
        public PaymentResult()
        {
            Success = true;
            Messages = new List<string>();
        }

        public List<string> Messages { get; set; }
        public bool Success { get; set; }
    }

    public class ShopApp
    {
        public ShopApp()
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
                CartAddItemResult result = new CartAddItemResult();               

                if (!ValidateAdd(item, result))
                {
                    return result;
                }

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

                return result;
            }            
        }

        private bool ValidateAdd(CartAddItem item, CartAddItemResult result)
        {   
            if (string.IsNullOrEmpty(item.CustomerId))
            {
                result.Success = false;
                result.Messages.Add("CustomerId is required.");
            }

            if (string.IsNullOrEmpty(item.ProductId))
            {
                result.Success = false;
                result.Messages.Add("ProductId is required.");
            }

            if (item.Number == 0)
            {
                result.Success = false;
                result.Messages.Add("Number is required.");
            }

            return result.Success;
        }

        public CartUpdateItemResult Update(CartUpdateItem item)
        {
            using (Transaction tran = _session.BegingTran())
            {
                CartUpdateItemResult result = new CartUpdateItemResult();

                if (!ValidateUpdate(item, result))
                {
                    return result;
                }

                CartModel cartModel = _cartRepository.Get(item.CustomerId);

                if (cartModel == null)
                {
                    return result;
                }

                CartItemModel itemModel = cartModel.CartItemList.FirstOrDefault(x => x.CartItemId == item.CartItemId);

                if (itemModel == null)
                {
                    return result;
                }

                itemModel.Number = item.Number;

                cartModel.UpdateItem(itemModel);

                _cartRepository.Save(cartModel);

                tran.Commit();

                return result;
            }
        }

        private bool ValidateUpdate(CartUpdateItem item, CartUpdateItemResult result)
        {
            if (string.IsNullOrEmpty(item.CustomerId))
            {
                result.Success = false;
                result.Messages.Add("CustomerId is required.");
            }

            if (string.IsNullOrEmpty(item.CartItemId))
            {
                result.Success = false;
                result.Messages.Add("CartItemId is required.");
            }

            if (item.Number == 0)
            {
                result.Success = false;
                result.Messages.Add("Number is required.");
            }

            return result.Success;
        }

        public CartRemoveItemResult Remove(CartRemoveItem item)
        {            
            using (Transaction tran = _session.BegingTran())
            {
                CartRemoveItemResult result = new CartRemoveItemResult();

                if (!ValidateRemove(item, result))
                {
                    return result;
                }

                CartModel cartModel = _cartRepository.Get(item.CustomerId);
                cartModel.RemoveItem(item.CartItemId);

                _cartRepository.Save(cartModel);

                tran.Commit();

                return result;
            }
        }

        private bool ValidateRemove(CartRemoveItem item, CartRemoveItemResult result)
        {
            if (string.IsNullOrEmpty(item.CustomerId))
            {
                result.Success = false;
                result.Messages.Add("CustomerId is required.");
            }

            if (string.IsNullOrEmpty(item.CartItemId))
            {
                result.Success = false;
                result.Messages.Add("CartItemId is required.");
            }

            return result.Success;
        }

        public Customer FindCustomer(string customerId)
        {
            using (_session.Open())
            {
                CustomerModel customerModel = _customerRepository.Find(customerId);

                return new Customer
                {
                    CustomerId = customerModel.CustomerId,
                    CustomerName = customerModel.CustomerName,
                    Address = customerModel.Address,
                    CreditCardNo = customerModel.CreditCardNo,
                    CreditCardHolder = customerModel.CreditCardHolder,
                    CreditCardExpirationDate = customerModel.CreditCardExpirationDate
                };
            }
        }

        public PaymentAmountInfo GetPaymentAmount(string customerId)
        {
            using (_session.Open())
            {
                CustomerModel customerModel = _customerRepository.Find(customerId);
                CartModel cartModel = _cartRepository.Get(customerId);
                decimal postage = _cartReader.GetPostage();

                PaymentModel paymentModel = cartModel.Checkout(customerModel.Address, postage);

                return new PaymentAmountInfo
                {
                    Postage = postage,
                    TotalPrice = cartModel.TotalPrice(),
                    PaymentAmount = paymentModel.PaymentAmount()
                };
            }
        }

        public PaymentResult Pay(Payment payment)
        {
            using (Transaction tran = _session.BegingTran())
            {
                PaymentResult result = new PaymentResult();

                if (!ValidatePay(payment, result))
                {
                    return result;
                }

                CartModel cartModel = _cartRepository.Get(payment.CustomerId);
                decimal postage = _cartReader.GetPostage();

                PaymentModel paymentModel = cartModel.Checkout(payment.ShippingAddress, postage);

                paymentModel.Pay(
                        payment.CreditCardNo,
                        payment.CreditCardHolder,
                        payment.CreditCardExpirationDate,
                        _creditCardService);

                _cartRepository.Save(cartModel);
                _paymentRepository.Save(paymentModel);

                tran.Commit();

                return result;
            }
        }

        private bool ValidatePay(Payment payment, PaymentResult result)
        {
            if (string.IsNullOrEmpty(payment.CustomerId))
            {
                result.Success = false;
                result.Messages.Add("CustomerId is required.");
            }

            if (string.IsNullOrEmpty(payment.CreditCardNo))
            {
                result.Success = false;
                result.Messages.Add("CreditCardNo is required.");
            }

            if (string.IsNullOrEmpty(payment.CreditCardHolder))
            {
                result.Success = false;
                result.Messages.Add("CreditCardHolder is required.");
            }

            if (string.IsNullOrEmpty(payment.CreditCardExpirationDate))
            {
                result.Success = false;
                result.Messages.Add("CreditCardExpirationDate is required.");
            }

            if (string.IsNullOrEmpty(payment.ShippingAddress))
            {
                result.Success = false;
                result.Messages.Add("ShippingAddress is required.");
            }

            return result.Success;
        }
    }
}
