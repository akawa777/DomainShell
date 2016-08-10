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
        
        public CustomerModel Customer { get; set; }
        public List<CartItemModel> CartItems { get; set; }

        public decimal TotalPrice()
        {
            return CartItems.Sum(x => x.Product.Price);
        }

        public State State { get; private set; }

        public void Create()
        {
            State = State.Created;
        }

        public void Update()
        {
            State = State.Updated;
        }

        public void  Delete()
        {
            State = State.Deleted;            
        }

        public void Accepted()
        {
            State = State.UnChanged;
        }

        public PaymentModel Checkout(string shippingAddress, IPostageService service)
        {
            PaymentModel payment = new PaymentModel();            
            payment.ShippingAddress = shippingAddress;
            payment.Postage = service.GetPostage(shippingAddress);
            
            foreach (CartItemModel item in CartItems)
            {
                payment.PaymentItemList.Add(new PaymentItemModel
                {
                    Product = item.Product,
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
        public ProductModel Product { get; set; }
        public int Number { get; set; }        
    }
}
