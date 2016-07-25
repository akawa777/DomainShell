using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DomainShell.Tests.Web.Controllers
{
    public class FacadeController : Controller
    {
        //
        // GET: /Facade/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Html()
        {
            string app = Request.RequestContext.RouteData.Values["app"].ToString();
            string html = Request.RequestContext.RouteData.Values["html"].ToString();

            return PartialView(string.Format(@"..\{0}\{1}", app, html));
        }
	}
}