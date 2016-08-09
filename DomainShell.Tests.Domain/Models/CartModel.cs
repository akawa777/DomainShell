using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain.Models
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
    }

    public class CartItemModel
    {
        public string CartId { get; set; }
        public string CartItemId { get; set; }
        public ProductModel Product { get; set; }
        public int Number { get; set; }        
    }
}
