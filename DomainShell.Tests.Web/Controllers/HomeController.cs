using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainShell.Tests.Web.ServiceLocators;
using DomainShell.Tests.Web.Models.Person;

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
            return View();
        }

        [HttpPost]
        public ActionResult List()
        {
            PersonListQuery query = new PersonListQuery();

            PersonData[] persons = _locator.QueryFacade.Get(query);

            return Json(persons);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(AddPersonCommand command)
        {
            bool success = false;

            _locator.CommandBus.Callback(command, result => success = result.Success);

            _locator.CommandBus.Send(command);

            return Json(success);
        }

        public ActionResult Detail(int id)
        {
            return View(id);
        }

        [HttpPost]
        public ActionResult Load(PersonQuery query)
        {
            PersonData person = _locator.QueryFacade.Get(query);

            return Json(person);
        }

        [HttpPost]
        public ActionResult Update(UpdatePersonCommand command)
        {
            bool success = false;

            _locator.CommandBus.Callback(command, result => success = result);

            _locator.CommandBus.Send(command);

            return Json(success);
        }

        [HttpPost]
        public ActionResult Remove(RemovePersonCommand command)
        {
            bool success = false;

            _locator.CommandBus.Callback(command, result => success = result);

            _locator.CommandBus.Send(command);

            return Json(success);
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