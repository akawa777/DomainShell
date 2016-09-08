using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainShell.Tests.Web.Controllers.Extension;
using DomainShell.Tests.App.Cart;

namespace DomainShell.Tests.Web.Controllers
{
    public class CartController : Controller
    {
        private CartApp _app = new CartApp();
        

        public ActionResult Update(UpdateCartItemCommand command)
        {
            UpdateCartItemResult result = _app.UpdateCartItem(command);

            return this.JsonCamel(result);
        }

        public ActionResult Remove(RemoveCartItemCommand command)
        {
            RemoveCartItemResult result = _app.RemoveCartItem(command);

            return this.JsonCamel(result);
        }

        public ActionResult Checkout(CheckoutCommand command)
        {
            CheckoutResult result = _app.Checkout(command);

            return this.JsonCamel(result);
        }

        public ActionResult GetCartItems(CartItemsQuery query)
        {
            CartItem[] items = _app.GetCartItems(query);

            return this.JsonCamel(items);
        }

        public ActionResult GetPaymentAmountInfo(PaymentAmountInfoQuery query)
        {
            PaymentAmountInfo info = _app.GetPaymentAmountInfo(query);

            return this.JsonCamel(info);
        }

        public ActionResult GetCustomer(CustomerQuery query)
        {
            Customer customer = _app.GetCustomer(query);

            return this.JsonCamel(customer);
        }
    }
}