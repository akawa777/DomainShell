using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainShell.Tests.Web.Controllers.Extension;
using DomainShell.Tests.Apps.Cart;

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

        public ActionResult Get(string customerId)
        {
            CartItem[] items = _app.Get(customerId);

            return this.JsonCamel(items);
        }

        public ActionResult Add(CartAddItem item)
        {
            CartAddItemResult result = _app.Add(item);

            return this.JsonCamel(result);
        }

        public ActionResult Update(CartUpdateItem item)
        {
            CartUpdateItemResult result = _app.Update(item);

            return this.JsonCamel(result);
        }

        public ActionResult Remove(CartRemoveItem item)
        {
            CartRemoveItemResult result = _app.Remove(item);

            return this.JsonCamel(result);
        }

        public ActionResult GetPaymentAmountInfo(string customerId)
        {
            PaymentAmountInfo info = _app.GetPaymentAmount(customerId);

            return this.JsonCamel(info);
        }

        public ActionResult Pay(Payment payment)
        {
            PaymentResult result = _app.Pay(payment);

            return this.JsonCamel(result);
        }

        public ActionResult FindCustomer(string customerId)
        {
            Customer customer = _app.FindCustomer(customerId);

            return this.JsonCamel(customer);
        }

        public ActionResult GetPaymentContents(string customerId)
        {
            PaymentContent[] paymentContens = _app.GetPaymentContents(customerId);

            return this.JsonCamel(paymentContens);
        }
    }
}