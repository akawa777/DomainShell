using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.Domain.Product;
using DomainShell.Tests.Domain.Purchase;

namespace DomainShell.Tests.Domain.Cart
{
    public class CartRecord
    {
        public string CartId { get; set; }
        public string CustomerId { get; set; }
        public List<CartItemRecord> CartItemList { get; set; }
    }

    public class CartModel : IAggregateRoot
    {
        public CartModel()          
        {
            CartItems = new ReadOnlyCollection<CartItemModel>(_cartItemList);
        }

        public CartModel(CartRecord record) : this()
        {
            CartId = record.CartId;
            CustomerId = record.CustomerId;

            foreach (CartItemRecord itemRecord in record.CartItemList)
            {
                CartItemModel item = new CartItemModel(itemRecord);
                _cartItemList.Add(item);
            }
        }

        public string CartId { get; set; }
        public string CustomerId { get; set; }

        public ReadOnlyCollection<CartItemModel> CartItems { get; set; }
        protected List<CartItemModel> _cartItemList = new List<CartItemModel>();        

        public decimal TotalPrice()
        {
            return CartItems.Sum(x => x.Product.Price * x.Number);
        }

        public void AddItem(CartItemModel item)
        {
            string cartItemId;
            if (CartItems.Count == 0)
            {                
                cartItemId = "1";
            }
            else
            {
                cartItemId = (CartItems.Max(x => int.Parse(x.CartItemId)) + 1).ToString();
            }
            
            item.CartId = CartId;
            item.CartItemId = cartItemId;
            item.ProductId = item.Product.ProductId;                        
            
            _cartItemList.Add(item);
        }

        public CartItemModel GetCartItem(string cartItemId)
        {
            CartItemModel item = CartItems.FirstOrDefault(x => x.CartItemId == cartItemId);

            return item;
        }

        public void RemoveItem(string cartItemId)
        {
            CartItemModel item = CartItems.FirstOrDefault(x => x.CartItemId == cartItemId);

            _cartItemList.Remove(item);
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
            decimal paymentAmount = GetPaymentAmount(postage, taxService);
            decimal Tax = GetTax(postage, taxService);

            PurchaseModel purchase = new PurchaseModel();
            purchase.CustomerId = CustomerId;
            purchase.ShippingAddress = shippingAddress;
            purchase.Postage = postage;
            purchase.Tax = GetTax(postage, taxService);
            purchase.PaymentAmount = paymentAmount;
            purchase.PaymentDate = DateTime.Now.ToString("yyyyMMddHHmmss");

            foreach (CartItemModel item in CartItems)
            {
                PurchaseDetailModel purchaseDetailModel = new PurchaseDetailModel
                {                    
                    ProductId = item.Product.ProductId,
                    Number = item.Number,
                    PriceAtTime = item.Product.Price
                };

                purchase.AddDetail(purchaseDetailModel);
            }            

            return purchase;            
        }        
    }

    public class CartItemRecord
    {
        public string CartId { get; set; }
        public string CartItemId { get; set; }
        public string ProductId { get; set; }
        public ProductRecord Product { get; set; }
        public int Number { get; set; }        
    }

    public class CartItemModel
    {
        public CartItemModel()
        {
            
        }

        public CartItemModel(CartItemRecord record)
        {
            CartId = record.CartId;
            CartItemId = record.CartItemId;
            ProductId = record.ProductId;

            ProductModel product = new ProductModel(record.Product);

            Number = record.Number;
        }

        public string CartId { get; set; }
        public string CartItemId { get; set; }
        public string ProductId { get; set; }
        public ProductModel Product { get; set; }
        public int Number { get; set; }
    }
}
