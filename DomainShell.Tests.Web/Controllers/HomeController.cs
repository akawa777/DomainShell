using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainShell.Tests.Web.ServiceLocators;
using DomainShell.Tests.Web.BizLogic.Person;

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

        public ActionResult New()
        {
            return View();
        }

        public ActionResult Create(AddPersonCommand command)
        {
            bool success = false;

            _locator.CommandBus.Callback(command, result => success = result.Success);

            _locator.CommandBus.Send(command);

            return RedirectToAction("index");
        }

        public ActionResult Detail(PersonQuery query)
        {
            PersonData person = _locator.QueryFacade.Get(query);

            return View(person);
        }

        public ActionResult Update(UpdatePersonCommand command)
        {
            bool success = false;

            _locator.CommandBus.Callback(command, result => success = result);

            _locator.CommandBus.Send(command);

            return RedirectToAction("index");
        }

        public ActionResult Delete(RemovePersonCommand command)
        {
            bool success = false;

            _locator.CommandBus.Callback(command, result => success = result);

            _locator.CommandBus.Send(command);

            return RedirectToAction("index");
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