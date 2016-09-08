using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainShell.Tests.Web.Controllers.Extension;
using DomainShell.Tests.App.Purchase;

namespace DomainShell.Tests.Web.Controllers
{
    public class PurchaseController : Controller
    {
        private PurchaseApp _app = new PurchaseApp();

        public ActionResult GetPurchaseHistories(PurchasesQuery query)
        {
            Purchase[] purchaseHistories = _app.GetPurchases(query);

            return this.JsonCamel(purchaseHistories);
        }
    }
}