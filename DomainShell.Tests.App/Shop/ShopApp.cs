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

namespace DomainShell.Tests.App.Shop
{
    public class ShopApp
    {
        public ShopApp()
        {
            _session = new Session();

            _cartReader = new CartReader(_session);
            _cartRepository = new CartRepository(_session);
            _customerRepository = new CustomerRepository(_session);
            _productRepository = new ProductRepository(_session);
            _paymentReader = new PaymentReader(_session);
            _paymentRepository = new PaymentRepository(_session);

            _creditCardService = new CreditCardService();            
        }

        private Session _session;

        private CartReader _cartReader;        
        private CartRepository _cartRepository;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;
        private PaymentReader _paymentReader;
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

        public CartAddItemResult Add(CartAddItemCommand item)
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

        private bool ValidateAdd(CartAddItemCommand item, CartAddItemResult result)
        {   
            if (string.IsNullOrEmpty(item.CustomerId))
            {
                result.Success = false;
                result.Messages.Add("CustomerId is required.");
            }

            if (_customerRepository.Find(item.CustomerId) == null)
            {
                result.Success = false;
                result.Messages.Add("not exist CustomerId.");
            }

            if (string.IsNullOrEmpty(item.ProductId))
            {
                result.Success = false;
                result.Messages.Add("ProductId is required.");
            }

            if (_productRepository.Find(item.ProductId) == null)
            {
                result.Success = false;
                result.Messages.Add("not exist ProductId.");
            }
            
            if (item.Number == 0)
            {
                result.Success = false;
                result.Messages.Add("Number is required.");
            }

            return result.Success;
        }

        public CartUpdateItemResult Update(CartUpdateItemCommand item)
        {
            using (Transaction tran = _session.BegingTran())
            {
                CartUpdateItemResult result = new CartUpdateItemResult();

                if (!ValidateUpdate(item, result))
                {
                    return result;
                }

                CartModel cartModel = _cartRepository.Get(item.CustomerId);
                CartItemModel itemModel = cartModel.GetCartItem(item.CartItemId);

                itemModel.Number = item.Number;

                cartModel.UpdateItem(itemModel);

                _cartRepository.Save(cartModel);

                tran.Commit();

                return result;
            }
        }

        private bool ValidateUpdate(CartUpdateItemCommand item, CartUpdateItemResult result)
        {
            if (string.IsNullOrEmpty(item.CustomerId))
            {
                result.Success = false;
                result.Messages.Add("CustomerId is required.");
            }

            if (_cartRepository.Get(item.CustomerId) == null)
            {
                result.Success = false;
                result.Messages.Add("not exist cart.");
            } 
            else if (_cartRepository.Get(item.CustomerId).GetCartItem(item.CartItemId) == null)
            {
                result.Success = false;
                result.Messages.Add("not exist cart item.");
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

        public CartRemoveItemResult Remove(CartRemoveItemCommand item)
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

        private bool ValidateRemove(CartRemoveItemCommand item, CartRemoveItemResult result)
        {
            if (string.IsNullOrEmpty(item.CustomerId))
            {
                result.Success = false;
                result.Messages.Add("CustomerId is required.");
            }

            if (_cartRepository.Get(item.CustomerId) == null)
            {
                result.Success = false;
                result.Messages.Add("not exist cart.");
            }
            else if (_cartRepository.Get(item.CustomerId).GetCartItem(item.CartItemId) == null)
            {
                result.Success = false;
                result.Messages.Add("not exist cart item.");
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

        public PaymentResult Pay(PaymentCommand payment)
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

        private bool ValidatePay(PaymentCommand payment, PaymentResult result)
        {
            if (string.IsNullOrEmpty(payment.CustomerId))
            {
                result.Success = false;
                result.Messages.Add("CustomerId is required.");
            }

            if (_cartRepository.Get(payment.CustomerId) == null)
            {
                result.Success = false;
                result.Messages.Add("not exist cart.");
            }
            else if (_cartRepository.Get(payment.CustomerId).CartItemList.Count == 0)
            {
                result.Success = false;
                result.Messages.Add("not has cart items.");
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

        public PaymentContent[] GetPaymentContents(string customerId)
        {
            using (_session.Open())
            {
                List<PaymentContentReadObject> readObjects = _paymentReader.GetPaymentContentList(customerId);

                return readObjects.Select(x => new PaymentContent
                {
                    PaymentId = x.PaymentId,
                    CustomerId = x.CustomerId,                    
                    PaymentDate = x.PaymentDate,
                    ShippingAddress = x.ShippingAddress,
                    PaymentAmount = x.PaymentAmount
                }).ToArray();
            }
        }
    }
}
