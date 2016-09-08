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
    }
}