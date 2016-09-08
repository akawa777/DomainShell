﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Domain.Cart;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.Domain.Product;
using DomainShell.Tests.Domain.Purchase;
using DomainShell.Infrastructure;
using DomainShell.Tests.Infrastructure;
using DomainShell.Tests.Infrastructure.Common;
using DomainShell.Tests.Infrastructure.Cart;
using DomainShell.Tests.Infrastructure.Customer;
using DomainShell.Tests.Infrastructure.Product;
using DomainShell.Tests.Infrastructure.Purchase;

namespace DomainShell.Tests.App.Cart
{
    public class CartApp
    {
        public CartApp()
        {
            _session = new Session(new SqliteSessionKernel());

            _idService = new IdService(_session);
            _cartReader = new CartReader(_session);            
            _cartRepository = new CartRepository(_session);
            _purchaseRepository = new PurchaseRepository(_session);
            _customerRepository = new CustomerRepository(_session);
            _productRepository = new ProductRepository(_session);            
            _taxService = new TaxService(_session);
            _creditCardService = new CreditCardService();            
        }

        private Session _session;
        private IdService _idService;
        private CartReader _cartReader;        
        private CartRepository _cartRepository;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;        
        private PurchaseRepository _purchaseRepository;
        private ITaxService _taxService;
        private ICreditCardService _creditCardService;                        

        public UpdateCartItemResult UpdateCartItem(UpdateCartItemCommand command)
        {
            using (ITran tran = _session.Tran())
            {
                UpdateCartItemResult result = new UpdateCartItemResult();

                if (!Validate(command, result))
                {
                    return result;
                }

                CartModel cartModel = _cartRepository.Get(command.CustomerId);

                CartItemModel cartItemModel = cartModel.GetCartItem(command.CartItemId);
                cartItemModel.Number = command.Number;

                _cartRepository.Save(cartModel);

                tran.Complete();

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
            using (ITran tran = _session.Tran())
            {
                RemoveCartItemResult result = new RemoveCartItemResult();

                if (!ValidateRemove(command, result))
                {
                    return result;
                }

                CartModel cartModel = _cartRepository.Get(command.CustomerId);

                cartModel.RemoveItem(command.CartItemId);

                _cartRepository.Save(cartModel);

                tran.Complete();

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

        public CheckoutResult Checkout(CheckoutCommand command)
        {
            using (ITran tran = _session.Tran())
            {
                CheckoutResult result = new CheckoutResult();

                if (!ValidateCheckout(command, result))
                {
                    return result;
                }

                decimal postage = _cartReader.GetPostage();
                CartModel cartModel = _cartRepository.Get(command.CustomerId);                     

                PurchaseModel purchaseModel = cartModel.Checkout(
                    command.CreditCardNo,
                    command.CreditCardHolder,
                    command.CreditCardExpirationDate,
                    command.ShippingAddress, 
                    postage, 
                    _taxService, 
                    _creditCardService,
                    _idService);                                

                _cartRepository.Save(cartModel);
                _purchaseRepository.Save(purchaseModel);

                tran.Complete();

                return result;
            }
        }

        private bool ValidateCheckout(CheckoutCommand command, CheckoutResult result)
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
            else if (_cartRepository.Get(command.CustomerId).CartItems.Count == 0)
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

        public CartItem[] GetCartItems(CartItemsQuery query)
        {
            using (_session.Connect())
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
            using (_session.Connect())
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
            using (_session.Connect())
            {
                decimal postage = _cartReader.GetPostage();
                CartModel cartModel = _cartRepository.Get(query.CustomerId);

                return new PaymentAmountInfo
                {
                    Postage = postage,
                    TotalPrice = cartModel.GetTotalPrice(),
                    Tax = cartModel.GetTax(postage, _taxService),
                    PaymentAmount = cartModel.GetPaymentAmount(postage, _taxService)
                };
            }
        }
    }
}
