﻿using System;
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
        public CartId(int cutomerId)
        {
            CutomerId = cutomerId;
        }

        public int CutomerId
        {
            get;
            protected set;
        }

        public string Value
        {
            get
            {
                return string.Join(":", CutomerId);
            }
        }
    }

    public class CartDetailId : IValue
    {
        public CartDetailId(int customerId, int cartItemNo)
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

    public class CartItemEntity : IEntity<CartDetailId>
    {
        public CartItemEntity(CartDetailId id)
        {
            Id = id;
        }

        public CartDetailId Id
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
        public CartEntity(CartId id)
        {
            Id = id;
        }

        private List<IDomainEvent> _events = new List<IDomainEvent>();
        private List<CartItemEntity> _cartItemList = new List<CartItemEntity>();

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
            CartItemEntity cartItem = new CartItemEntity(new CartDetailId(Id.CutomerId, _cartItemList.Max(x => x.Id.CartItemNo) + 1));

            cartItem.ProductId = productId;
            cartItem.Quantity = quantity;

            _cartItemList.Add(cartItem);
        }

        public void RemoveProduct(CartDetailId cartDetailId)
        {
            CartItemEntity cartItem = _cartItemList.FirstOrDefault(x => x.Id.Value == cartDetailId.Value);

            if (cartItem == null)
            {
                return;
            }

            _cartItemList.Remove(cartItem);
        }

        protected decimal GetTotalPrice(IProductReadService productReadService)
        {
            return _cartItemList.Sum(x => productReadService.Find(x.ProductId).Price * x.Quantity);
        }

        public virtual void Purchase(CreditCardValue creditCard, ICreditCardService creditCardService, IProductReadService productReadService, IValidationSpec<CartEntity> spec)
        {
            decimal totalPrice = GetTotalPrice(productReadService);
            string content = productReadService.Find(_cartItemList[0].ProductId).ProductName;

            Validate(spec);

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
                CustomerId = Id.CutomerId,                
                PucharseDtoList = list
            };

            _events.Add(@event);
        }

        public virtual void Validate(IValidationSpec<CartEntity> spec)
        {
            string[] errors;
            if (!spec.Validate(this, out errors))
            {
                throw new Exception(errors[0]);
            }
        }

        public virtual void Delete()
        {
            
        }
    }
}
