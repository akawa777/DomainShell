using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain.Common;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.Domain.Product;
using DomainShell.Tests.Domain.Purchase;

namespace DomainShell.Tests.Domain.Cart
{
    public class CartModel : IAggregateRoot
    {
        private CartModel()
        {
            CartItems = new ReadOnlyCollection<CartItemModel>(_cartItemList);
        }

        public CartModel(IIdService idService)
            : this()
        {
            CartId = idService.CreateId<CartModel>();

            State = State.Added;
        }        

        public CartModel(CartProxy proxy)
            : this()
        {
            CartId = proxy.CartId;
            Customer = new CustomerModel(proxy.Customer);

            foreach (CartItemProxy itemProxy in proxy.CartItemList)
            {
                CartItemModel item = new CartItemModel(itemProxy);
                _cartItemList.Add(item);
            }

            State = State.Stored;
        }

        public State State { get; private set; }

        public void Stored()
        {
            State = State.Stored;
        }

        public string CartId { get; private set; }
        public string CustomerId { get { return Customer.CustomerId; } }
        public CustomerModel Customer { get; set; }

        public ReadOnlyCollection<CartItemModel> CartItems { get; set; }
        protected List<CartItemModel> _cartItemList = new List<CartItemModel>();
        
        public CartItemModel CreateDetail()
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

            CartItemModel item = new CartItemModel(CartId, cartItemId);

            _cartItemList.Add(item);

            return item;
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

        public decimal GetTotalPrice()
        {
            return CartItems.Sum(x => x.Product.Price * x.Number);
        }

        public decimal GetTax(decimal postage, ITaxService taxService)
        {
            return taxService.GetTax(postage + GetTotalPrice());
        }

        public decimal GetPaymentAmount(decimal postage, ITaxService taxService)
        {
            return taxService.Calculate(postage + GetTotalPrice());
        }

        public PurchaseModel Checkout(
            string creditCardNo,
            string creditCardHolder,
            string creditCardExpirationDate,
            string shippingAddress, 
            decimal postage, 
            ITaxService taxService, 
            ICreditCardService creditCardService,
            IIdService idService)
        {   
            decimal paymentAmount = GetPaymentAmount(postage, taxService);
            decimal Tax = GetTax(postage, taxService);

            PurchaseModel purchase = new PurchaseModel(idService);
            purchase.Customer = Customer;
            purchase.CreditCardNo = creditCardNo;
            purchase.CreditCardHolder = creditCardHolder;
            purchase.CreditCardExpirationDate = creditCardExpirationDate;
            purchase.ShippingAddress = shippingAddress;
            purchase.Postage = postage;
            purchase.Tax = GetTax(postage, taxService);
            purchase.PaymentAmount = paymentAmount;
            purchase.PaymentDate = DateTime.Now.ToString("yyyyMMddHHmmss");

            foreach (CartItemModel item in CartItems)
            {
                PurchaseDetailModel purchaseDetailModel = purchase.CreateDetail();

                purchaseDetailModel.Product = item.Product;
                purchaseDetailModel.Number = item.Number;
                purchaseDetailModel.PriceAtTime = item.Product.Price;                
            }

            creditCardService.Pay(creditCardNo, creditCardHolder, creditCardExpirationDate, paymentAmount);

            _cartItemList.Clear();

            return purchase;            
        }
    }

    public class CartItemModel
    {
        private CartItemModel()
        {
            
        }

        public CartItemModel(string cartId, string cartItemId)
        {
            CartId = cartId;
            CartItemId = cartItemId;
        }

        public CartItemModel(CartItemProxy proxy)
        {
            CartId = proxy.CartId;
            CartItemId = proxy.CartItemId;            
            Product = new ProductModel(proxy.Product);
            Number = proxy.Number;
        }

        public string CartId { get; private set; }
        public string CartItemId { get; private set; }
        public string ProductId { get { return Product.ProductId;  } }
        public ProductModel Product { get; set; }
        public int Number { get; set; }
    }
}
