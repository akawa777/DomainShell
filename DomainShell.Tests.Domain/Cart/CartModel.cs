using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.Domain.Product;
using DomainShell.Tests.Domain.Payment;

namespace DomainShell.Tests.Domain.Cart
{
    public class CartModel : IAggregateRoot
    {
        public string CartId { get; set; }
        public string CustomerId { get; set; }        
        public List<CartItemModel> CartItemList { get; set; }

        public decimal TotalPrice()
        {
            return CartItemList.Sum(x => x.Product.Price);
        }

        public State State { get; private set; }

        public void Create(string customerId)
        {
            if (!string.IsNullOrEmpty(CartId))
            {
                throw new Exception("already created.");
            }

            CustomerId = customerId;
            CartItemList = new List<CartItemModel>();

            State = State.Created;
        }

        public void AddItem(CartItemModel item)
        {
            if (!string.IsNullOrEmpty(CartId))
            {
                State = State.Updated;
            }

            item.CartItemId = (CartItemList.Count + 1).ToString();

            CartItemList.Add(item);
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

            State = State.Updated;            
        }

        public void Accepted()
        {
            State = State.UnChanged;
        }

        public PaymentModel Checkout(string shippingAddress, decimal postage)
        {
            if (CartItemList == null || CartItemList.Count == 0)
            {
                throw new Exception("not exists CartItems");
            }

            PaymentModel payment = new PaymentModel();
            payment.CustomerId = CustomerId;            
            payment.ShippingAddress = shippingAddress;
            payment.Postage = postage;
            payment.PaymentItemList = new List<PaymentItemModel>();

            foreach (CartItemModel item in CartItemList)
            {
                payment.PaymentItemList.Add(new PaymentItemModel
                {
                    ProductId = item.Product.ProductId,
                    Number = item.Number,
                    PriceAtTime = item.Product.Price
                });
            }

            State = State.Deleted;

            return payment;            
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
