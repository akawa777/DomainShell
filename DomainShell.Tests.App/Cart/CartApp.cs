using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Domain.Cart;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.Domain.Product;
using DomainShell.Tests.Infrastructure;
using DomainShell.Tests.Infrastructure.Cart;
using DomainShell.Tests.Infrastructure.Customer;
using DomainShell.Tests.Infrastructure.Product;

namespace DomainShell.Tests.Apps.Cart
{
    public class CartData
    {
        public string CartId { get; set; }
        public string MainProductName { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalNumber { get; set; }
    }

    public class CartItemData
    {
        public string CartId { get; set; }
        public string CartItemId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Number { get; set; }
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
        }

        private Session _session;
        private CartReader _cartReader;
        private CartRepository _cartRepository;
        private CustomerRepository _customerRepository;
        private ProductRepository _productRepository;

        public List<CartData> GetAll()
        {
            List<CartReadModel> cartList = _cartReader.GetAll();

            return cartList.Select(x => new CartData
            {
                CartId = x.CartId,
                MainProductName = x.MainProductName,
                TotalPrice = x.TotalPrice,
                TotalNumber = x.TotalNumber
            }).ToList();
        }

        public List<CartItemData> GetDetailList(string cartId)
        {
            List<CartItemReadModel> cartList = _cartReader.GetDetailList(cartId);

            return cartList.Select(x => new CartItemData
            {
                CartId = x.CartId,
                CartItemId = x.CartItemId,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Price = x.Price,
                Number = x.Number
            }).ToList();
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
    }
}
