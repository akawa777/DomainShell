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
        public List<CartItemModel> CartItems { get; set; }

        public decimal TotalPrice()
        {
            return CartItems.Sum(x => x.Product.Price);
        }

        public State State { get; private set; }

        public void Create(string customerId, List<CartItemModel> items)
        {
            if (!string.IsNullOrEmpty(CartId))
            {
                throw new Exception("already created.");
            }

            CustomerId = customerId;

            CartItems = items;
            State = State.Created;
        }

        public void Update(List<CartItemModel> items)
        {
            if (string.IsNullOrEmpty(CartId))
            {
                throw new Exception("not yet created.");
            }

            CartItems = items;
            State = State.Updated;
        }

        public void  Delete()
        {
            if (string.IsNullOrEmpty(CartId))
            {
                throw new Exception("not yet created.");
            }

            State = State.Deleted;            
        }

        public void Accepted()
        {
            State = State.UnChanged;
        }

        public PaymentModel Checkout(string shippingAddress, decimal postage)
        {
            if (CartItems == null || CartItems.Count == 0)
            {
                throw new Exception("not exists CartItems");
            }

            PaymentModel payment = new PaymentModel();            
            payment.ShippingAddress = shippingAddress;
            payment.Postage = postage;
            payment.PaymentItemList = new List<PaymentItemModel>();

            foreach (CartItemModel item in CartItems)
            {
                payment.PaymentItemList.Add(new PaymentItemModel
                {
                    ProductId = item.Product.ProductId,
                    Number = item.Number,
                    PriceAtTime = item.Product.Price
                });
            }

            Delete();            

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
