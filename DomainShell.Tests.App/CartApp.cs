using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Infrastructure;
using DomainShell.Tests.Domain.ReadModels;
using DomainShell.Tests.Infrastructure.Readers;
using DomainShell.Tests.Infrastructure.Repositries;
using DomainShell.Tests.Domain.Models;

namespace DomainShell.Tests.Apps
{
    public class CartData
    {
        public string CartId { get; set; }
        public string MainProductName { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalNumber { get; set; }
    }

    public class CartDetailData
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

        public List<CartDetailData> GetDetailList(string cartId)
        {
            List<CartDetailReadModel> cartList = _cartReader.GetDetailList(cartId);

            return cartList.Select(x => new CartDetailData
            {
                CartId = x.CartId,
                CartItemId = x.CartItemId,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Price = x.Price,
                Number = x.Number
            }).ToList();
        }

        public void Create(string customerId, CartDetailData[] details)
        {
            using (_session.Open())
            using (Transaction tran = _session.BegingTran())
            {
                CartModel cart = new CartModel();
                cart.Customer = _customerRepository.Get(customerId);

                cart.CartItems = new List<CartItemModel>();

                foreach (CartDetailData detail in details)
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

        public void Update(CartDetailData[] details)
        {
            using (_session.Open())
            using (Transaction tran = _session.BegingTran())
            {
                CartModel cart = _cartRepository.Get(details[0].CartId);                

                cart.CartItems = new List<CartItemModel>();

                foreach (CartDetailData detail in details)
                {
                    CartItemModel item = new CartItemModel();

                    item.Number = detail.Number;
                    item.Product = _productRepository.Get(detail.ProductId);

                    cart.CartItems.Add(item);
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
