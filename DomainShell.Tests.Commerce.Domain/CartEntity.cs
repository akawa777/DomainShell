using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;

namespace DomainShell.Tests.Commerce.Domain
{
    public class CartId : IValue
    {
        public CartId(int customerId)
        {
            CustomerId = customerId;
        }

        public int CustomerId
        {
            get;
            protected set;
        }

        public string Value
        {
            get
            {
                return string.Join(":", CustomerId);
            }
        }
    }

    public class CartItemId : IValue
    {
        public CartItemId(int customerId, int cartItemNo)
        {
            CartId = new CartId(customerId);
            CartItemNo = cartItemNo;
        }

        public CartId CartId
        {
            get;
            protected set;
        }

        public int CartItemNo
        {
            get;
            protected set;
        }

        public string Value
        {
            get
            {
                return string.Join(":", CartId.Value, CartItemNo);
            }
        }
    }

    public class CartItemEntity : IEntity<CartItemId>
    {
        public CartItemEntity(int customerId, int cartItemNo)
        {
            Id = new CartItemId(customerId, cartItemNo);
        }

        public CartItemId Id
        {
            get;
            protected set;
        }

        public int ProductId
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }
    }

    public class CartEntity : IAggregateRoot<CartId>
    {   
        public CartEntity(int customerId)
        {
            Id = new CartId(customerId);
        }

        protected List<IDomainEvent> _events = new List<IDomainEvent>();
        protected List<CartItemEntity> _cartItemList = new List<CartItemEntity>();

        public IEnumerable<IDomainEvent> GetEvents()
        {
            return _events;
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        public CartId Id
        {
            get;
            protected set;
        }

        public IReadOnlyList<CartItemEntity> CartItemList
        {
            get;
            protected set;
        }

        public void AddProduct(int productId, int quantity)
        {
            CartItemEntity cartItem = new CartItemEntity(Id.CustomerId, _cartItemList.Max(x => x.Id.CartItemNo) + 1);

            cartItem.ProductId = productId;
            cartItem.Quantity = quantity;

            _cartItemList.Add(cartItem);
        }

        public void RemoveProduct(CartItemId cartItemId)
        {
            CartItemEntity cartItem = _cartItemList.FirstOrDefault(x => x.Id.Value == cartItemId.Value);

            if (cartItem == null)
            {
                return;
            }

            _cartItemList.Remove(cartItem);
        }

        public virtual void Purchase(CreditCardValue creditCard, ICreditCardService creditCardService, IProductReadService productReadService, IValidationSpec<CartEntity, string> spec)
        {
            Validate(spec);

            decimal totalPrice = _cartItemList.Sum(x => productReadService.Find(x.ProductId).Price * x.Quantity);
            string content = productReadService.Find(_cartItemList[0].ProductId).ProductName;

            creditCardService.Pay(creditCard.CardCompanyId, creditCard.CardNo, totalPrice, content);

            Delete();

            List<PucharseDto> list = new List<PucharseDto>();

            foreach (CartItemEntity cartItem in _cartItemList)
            {
                PucharseDto dto = new PucharseDto
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity
                };

                list.Add(dto);
            }

            CartPurchasedEvent @event = new CartPurchasedEvent
            {
                CustomerId = Id.CustomerId,
                PucharseDtoList = list
            };

            _events.Add(@event);
        }

        public virtual void Validate(IValidationSpec<CartEntity, string> spec)
        {
            string[] errors;
            if (!spec.Validate(this, out errors))
            {
                throw new Exception(string.Join(Environment.NewLine, errors));
            }
        }

        protected virtual void Delete()
        {
            
        }
    }
}
