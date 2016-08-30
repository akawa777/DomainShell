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
    public class CartModel : IAggregateRoot
    {
        public CartModel()
        {            
            CartItemList = new ReadOnlyCollection<CartItemModel>(_cartItemList);
        }        

        public string CartId { get; set; }
        public string CustomerId { get; set; }

        public ReadOnlyCollection<CartItemModel> CartItemList { get; set; }
        private List<CartItemModel> _cartItemList = new List<CartItemModel>();

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

            State = State.Modified;
        }

        public void AddItem(CartItemModel item)
        {
            if (CartItemList.Any(x => x == item))
            {
                throw new Exception("already exist in CartItemList.");
            }

            if (CartItemList.Count == 0)
            {                
                item.CartItemId = "1";
            }
            else
            {
                item.CartItemId = (CartItemList.Max(x => int.Parse(x.CartItemId)) + 1).ToString();
            }

            item.ProductId = item.Product.ProductId;

            _cartItemList.Add(item);

            State = State.Modified;
        }

        public CartItemModel GetCartItem(string cartItemId)
        {
            CartItemModel item = CartItemList.FirstOrDefault(x => x.CartItemId == cartItemId);

            return item;
        }

        public void UpdateItem(CartItemModel item)
        {
            if (!CartItemList.Any(x => x == item))
            {
                throw new Exception("not exist in CartItemList.");
            }

            State = State.Modified;
        }

        public void RemoveItem(string cartItemId)
        {
            CartItemModel item = CartItemList.FirstOrDefault(x => x.CartItemId == cartItemId);

            if (item == null)
            {
                throw new Exception("not exist in CartItemList.");
            }

            _cartItemList.Remove(item);

            State = State.Deleted;
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

        public State State { get; private set; }

        public void Accepted()
        {
            State = State.UnChanged;
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
