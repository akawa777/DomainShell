using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainShell.Tests.Apps.Cart;
using System.IO;

namespace DomainShell.Tests.Web.Controllers
{
    public class CartController : Controller
    {
        private CartApp _app = new CartApp();

        public ActionResult GetProducts()
        {
            Product[] products = _app.GetProducts();

            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(string customerId)
        {
            CartItem[] items = _app.Get(customerId);

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(CartAddItem item)
        {
            CartAddItemResult result = _app.Add(item);

            return Json(result);
        }

        public ActionResult Remove(CartRemoveItem item)
        {
            CartRemoveItemResult result = _app.Remove(item);

            return Json(result);
        }

        public ActionResult GetPaymentAmount(PaymentAmountQuery query)
        {
            decimal paymentAmount = _app.GetPaymentAmount(query);

            return Json(paymentAmount);
        }

        public ActionResult Pay(Payment payment)
        {
            PaymentResult result = _app.Pay(payment);

            return Json(result);
        }
    }
}