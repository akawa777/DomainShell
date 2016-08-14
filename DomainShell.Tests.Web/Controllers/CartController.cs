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

        public ActionResult Get(string customerId)
        {
            CartItem[] items = _app.Get(customerId);

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(string customerId, CartItem item)
        {
            _app.Add(customerId, item);

            return Json(null);
        }

        public ActionResult Remove(string customerId, CartItem item)
        {
            _app.Remove(customerId, item);

            return Json(null);
        }

        public ActionResult Pay(Payment payment)
        {
            _app.Pay(payment);

            return Json(null);
        }
    }
}