﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.Domain.Product;
using DomainShell.Tests.Domain.Purchase;

namespace DomainShell.Tests.Domain.Cart
{
    public class CartModel : IAggregateRoot
    {
        public CartModel()
        {
            State = new State();
        }

        public State State { get; private set; }


        public string CartId { get; set; }
        public string CustomerId { get; set; }

        public List<CartItemModel> CartItemList { get; set; }        

        public decimal TotalPrice()
        {
            return CartItemList.Sum(x => x.Product.Price * x.Number);
        }

        public void Create()
        {
            if (!string.IsNullOrEmpty(CartId))
            {
                throw new Exception("already created.");
            }

            if (string.IsNullOrEmpty(CustomerId))
            {
                throw new Exception("CustomerId required.");
            }
            
            CartItemList = new List<CartItemModel>();            

            State.New();
        }

        public void AddItem(CartItemModel item)
        {
            if (CartItemList.Any(x => x == item))
            {
                throw new Exception("already exist in CartItemList.");
            }

            if (CartItemList == null || CartItemList.Count == 0)
            {
                CartItemList = new List<CartItemModel>();
                item.CartItemId = "1";
            }
            else
            {
                item.CartItemId = (CartItemList.Max(x => int.Parse(x.CartItemId)) + 1).ToString();
            }

            item.ProductId = item.Product.ProductId;

            CartItemList.Add(item);

            State.Modified();
        }

        public CartItemModel GetCartItem(string cartItemId)
        {
            CartItemModel item = CartItemList.FirstOrDefault(x => x.CartItemId == cartItemId);

            return item;
        }

        public void UpdateItem(CartItemModel item)
        {
            if (string.IsNullOrEmpty(CartId))
            {
                throw new Exception("not yet created.");
            }

            if (!CartItemList.Any(x => x == item))
            {
                throw new Exception("not exist in CartItemList.");
            }

            State.Modified();
        }

        public void RemoveItem(string cartItemId)
        {
            if (string.IsNullOrEmpty(CartId))
            {
                throw new Exception("not yet created.");
            }

            CartItemModel item = CartItemList.FirstOrDefault(x => x.CartItemId == cartItemId);

            if (item == null)
            {
                return;
            }

            CartItemList.Remove(item);

            State.Modified();        
        }

        public decimal GetTax(decimal postage, ITaxService taxService)
        {
            return taxService.GetTax(postage + TotalPrice());
        }

        public decimal GetPaymentAmount(decimal postage, ITaxService taxService)
        {
            return taxService.Calculate(postage + TotalPrice());
        }

        public PurchaseModel Checkout(string shippingAddress, decimal postage, ITaxService taxService)
        {
            if (CartItemList == null || CartItemList.Count == 0)
            {
                throw new Exception("not exists CartItems");
            }

            if (string.IsNullOrEmpty(shippingAddress))
            {
                throw new Exception("shippingAddress required.");
            }
            
            decimal paymentAmount = GetPaymentAmount(postage, taxService);
            decimal Tax = GetTax(postage, taxService);

            PurchaseModel purchase = new PurchaseModel();
            purchase.CustomerId = CustomerId;
            purchase.ShippingAddress = shippingAddress;
            purchase.Postage = postage;
            purchase.Tax = GetTax(postage, taxService);
            purchase.PaymentAmount = paymentAmount;
            purchase.PaymentDate = DateTime.Now.ToString("yyyyMMddHHmmss");            
            
            foreach (CartItemModel item in CartItemList)
            {
                PurchaseDetailModel purchaseDetail = new PurchaseDetailModel
                {                    
                    ProductId = item.Product.ProductId,
                    Number = item.Number,
                    PriceAtTime = item.Product.Price
                };

                purchase.AddDetail(purchaseDetail);
            }

            return purchase;            
        }
    }

    public class CartItemModel
    {
        public string CartId { get; set; }
        public string CartItemId { get; set; }
        public string ProductId { get; set; }
        public ProductModel Product { get; set; }
        public int Number { get; set; }        
    }
}
