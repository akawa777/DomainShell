using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainShell.Tests.Web.Controllers.Extension;
using DomainShell.Tests.App.Shop;

namespace DomainShell.Tests.Web.Controllers
{
    public class ShopController : Controller
    {
        private ShopApp _app = new ShopApp();

        public ActionResult GetProducts()
        {
            Product[] products = _app.GetProducts();

            return this.JsonCamel(products);
        }        

        public ActionResult Add(AddCartItemCommand command)
        {
            AddCartItemResult result = _app.AddCartItem(command);

            return this.JsonCamel(result);
        }

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

        public ActionResult Pay(PayCommand command)
        {
            PayResult result = _app.Pay(command);

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

        public ActionResult GetPayments(PaymentsQuery query)
        {
            Payment[] payments = _app.GetPayments(query);

            return this.JsonCamel(payments);
        }
    }
}