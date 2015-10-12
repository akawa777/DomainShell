using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainShell.Tests.Web.ServiceLocators;
using DomainShell.Tests.Web.BizLogic;

namespace DomainShell.Tests.Web.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(HomeServiceLocator locator)
        {
            _locator = locator;
        }

        private HomeServiceLocator _locator;

        public ActionResult Index()
        {
            PersonListQuery query = new PersonListQuery();

            PersonData[] persons = _locator.QueryFacade.Get(query);

            return View(persons);
        }

        public ActionResult Detail(int id)
        {
            PersonQuery query = new PersonQuery();

            PersonData person = _locator.QueryFacade.Get(query);

            return View(person);
        }

        public ActionResult Create(AddPersonCommand command)
        {
            bool success = false;

            _locator.CommandBus.Callback(command, result => success = result.Success);

            _locator.CommandBus.Send(command);

            return View(success);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}