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
            _taxService = new TaxService(_session);
            _creditCardService = new CreditCardService();            
        }

        private Session _session;
        private CartReader _cartReader;        
        private CartRepository _cartRepository;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;
        private PaymentReader _paymentReader;
        private PaymentRepository _paymentRepository;
        private ITaxService _taxService;
        private ICreditCardService _creditCardService;                

        public AddCartItemResult AddCartItem(AddCartItemCommand command)
        {
            using (Transaction tran = _session.BegingTran())
            {
                AddCartItemResult result = new AddCartItemResult();

                if (!Validate(command, result))
                {
                    return result;
                }

                CartModel cartModel = _cartRepository.Get(command.CustomerId);

                if (cartModel == null)
                {
                    cartModel = new CartModel();
                    cartModel.Create(command.CustomerId);
                }

                cartModel.AddItem(_productRepository.Find(command.ProductId), command.Number);

                _cartRepository.Save(cartModel);

                tran.Commit();

                return result;
            }            
        }

        private bool Validate(AddCartItemCommand command, AddCartItemResult result)
        {
            if (string.IsNullOrEmpty(command.CustomerId))
            {
                result.Success = false;
                result.Messages.Add("CustomerId is required.");
            }

            if (_customerRepository.Find(command.CustomerId) == null)
            {
                result.Success = false;
                result.Messages.Add("not exist CustomerId.");
            }

            if (string.IsNullOrEmpty(command.ProductId))
            {
                result.Success = false;
                result.Messages.Add("ProductId is required.");
            }

            if (_productRepository.Find(command.ProductId) == null)
            {
                result.Success = false;
                result.Messages.Add("not exist ProductId.");
            }

            if (command.Number == 0)
            {
                result.Success = false;
                result.Messages.Add("Number is required.");
            }

            return result.Success;
        }

        public UpdateCartItemResult UpdateCartItem(UpdateCartItemCommand command)
        {
            using (Transaction tran = _session.BegingTran())
            {
                UpdateCartItemResult result = new UpdateCartItemResult();

                if (!Validate(command, result))
                {
                    return result;
                }

                CartModel cartModel = _cartRepository.Get(command.CustomerId);

                cartModel.UpdateItem(command.CartItemId, command.Number);

                _cartRepository.Save(cartModel);

                tran.Commit();

                return result;
            }
        }

        private bool Validate(UpdateCartItemCommand command, UpdateCartItemResult result)
        {
            if (string.IsNullOrEmpty(command.CustomerId))
            {
                result.Success = false;
                result.Messages.Add("CustomerId is required.");
            }

            if (_cartRepository.Get(command.CustomerId) == null)
            {
                result.Success = false;
                result.Messages.Add("not exist cart.");
            }
            else if (_cartRepository.Get(command.CustomerId).GetCartItem(command.CartItemId) == null)
            {
                result.Success = false;
                result.Messages.Add("not exist cart item.");
            }

            if (string.IsNullOrEmpty(command.CartItemId))
            {
                result.Success = false;
                result.Messages.Add("CartItemId is required.");
            }

            if (command.Number == 0)
            {
                result.Success = false;
                result.Messages.Add("Number is required.");
            }

            return result.Success;
        }

        public RemoveCartItemResult RemoveCartItem(RemoveCartItemCommand command)
        {            
            using (Transaction tran = _session.BegingTran())
            {
                RemoveCartItemResult result = new RemoveCartItemResult();

                if (!ValidateRemove(command, result))
                {
                    return result;
                }

                CartModel cartModel = _cartRepository.Get(command.CustomerId);
                cartModel.RemoveItem(command.CartItemId);

                _cartRepository.Save(cartModel);

                tran.Commit();

                return result;
            }
        }

        private bool ValidateRemove(RemoveCartItemCommand command, RemoveCartItemResult result)
        {
            if (string.IsNullOrEmpty(command.CustomerId))
            {
                result.Success = false;
                result.Messages.Add("CustomerId is required.");
            }

            if (_cartRepository.Get(command.CustomerId) == null)
            {
                result.Success = false;
                result.Messages.Add("not exist cart.");
            }
            else if (_cartRepository.Get(command.CustomerId).GetCartItem(command.CartItemId) == null)
            {
                result.Success = false;
                result.Messages.Add("not exist cart item.");
            }

            if (string.IsNullOrEmpty(command.CartItemId))
            {
                result.Success = false;
                result.Messages.Add("CartItemId is required.");
            }

            return result.Success;
        }        

        public PayResult Pay(PayCommand command)
        {
            using (Transaction tran = _session.BegingTran())
            {
                PayResult result = new PayResult();

                if (!ValidatePay(command, result))
                {
                    return result;
                }

                decimal postage = _cartReader.GetPostage();
                CartModel cartModel = _cartRepository.Get(command.CustomerId);     

                PaymentModel paymentModel = cartModel.Checkout(command.ShippingAddress, postage, _taxService);

                paymentModel.Pay(
                        new CreditCardValue {
                            CreditCardNo = command.CreditCardNo,
                            CreditCardHolder = command.CreditCardHolder,
                            CreditCardExpirationDate = command.CreditCardExpirationDate,
                        },
                        _creditCardService);

                _cartRepository.Save(cartModel);
                _paymentRepository.Save(paymentModel);

                tran.Commit();

                return result;
            }
        }

        private bool ValidatePay(PayCommand command, PayResult result)
        {
            if (string.IsNullOrEmpty(command.CustomerId))
            {
                result.Success = false;
                result.Messages.Add("CustomerId is required.");
            }

            if (_cartRepository.Get(command.CustomerId) == null)
            {
                result.Success = false;
                result.Messages.Add("not exist cart.");
            }
            else if (_cartRepository.Get(command.CustomerId).CartItemList.Count == 0)
            {
                result.Success = false;
                result.Messages.Add("not has cart items.");
            }

            if (string.IsNullOrEmpty(command.CreditCardNo))
            {
                result.Success = false;
                result.Messages.Add("CreditCardNo is required.");
            }

            if (string.IsNullOrEmpty(command.CreditCardHolder))
            {
                result.Success = false;
                result.Messages.Add("CreditCardHolder is required.");
            }

            if (string.IsNullOrEmpty(command.CreditCardExpirationDate))
            {
                result.Success = false;
                result.Messages.Add("CreditCardExpirationDate is required.");
            }

            if (string.IsNullOrEmpty(command.ShippingAddress))
            {
                result.Success = false;
                result.Messages.Add("ShippingAddress is required.");
            }

            return result.Success;
        }

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

        public CartItem[] GetCartItems(CartItemsQuery query)
        {
            using (_session.Open())
            {
                CartItemReadObject[] readObjects = _cartReader.GetCartItems(query.CustomerId);

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

        public Customer GetCustomer(CustomerQuery query)
        {
            using (_session.Open())
            {
                CustomerModel customerModel = _customerRepository.Find(query.CustomerId);

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

        public PaymentAmountInfo GetPaymentAmountInfo(PaymentAmountInfoQuery query)
        {
            using (_session.Open())
            {
                decimal postage = _cartReader.GetPostage();                
                CartModel cartModel = _cartRepository.Get(query.CustomerId);                

                return new PaymentAmountInfo
                {
                    Postage = postage,
                    TotalPrice = cartModel.TotalPrice(),
                    Tax = cartModel.GetTax(postage, _taxService),
                    PaymentAmount = cartModel.GetPaymentAmount(postage, _taxService)
                };
            }
        }

        public Payment[] GetPayments(PaymentsQuery query)
        {
            using (_session.Open())
            {
                PaymentReadObject[] readObjects = _paymentReader.GetPayments(query.CustomerId);

                return readObjects.Select(x => new Payment
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
