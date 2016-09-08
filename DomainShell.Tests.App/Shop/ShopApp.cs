using System;
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

namespace DomainShell.Tests.App.Shop
{
    public class ShopApp
    {
        public ShopApp()
        {
            _session = new Session(new SqliteSessionKernel());

            _idService = new IdService(_session);
            _cartRepository = new CartRepository(_session);
            _customerRepository = new CustomerRepository(_session);
            _productRepository = new ProductRepository(_session);            
        }

        private Session _session;
        private IdService _idService;        
        private CartRepository _cartRepository;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;        

        public AddCartItemResult AddCartItem(AddCartItemCommand command)
        {
            using (ITran tran = _session.Tran())
            {
                AddCartItemResult result = new AddCartItemResult();

                if (!Validate(command, result))
                {
                    return result;
                }          

                CartModel cartModel = _cartRepository.Get(command.CustomerId);

                if (cartModel == null)
                {
                    cartModel = new CartModel(_idService);
                    cartModel.Customer = _customerRepository.Find(command.CustomerId);
                }

                CartItemModel cartItemModel = cartModel.AddDetail();

                cartItemModel.Product = _productRepository.Find(command.ProductId);
                cartItemModel.Number = command.Number;

                _cartRepository.Save(cartModel);

                result.CartItemId = cartItemModel.CartItemId;

                tran.Complete();                

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

        public Product[] GetProducts()
        {
            using (_session.Connect())
            {
                List<ProductModel> productList = _productRepository.GetAll();

                return productList
                    .Select(x => new Product { ProductId = x.ProductId, ProductName = x.ProductName, Price = x.Price })
                    .ToArray();
            }
        }
    }
}
