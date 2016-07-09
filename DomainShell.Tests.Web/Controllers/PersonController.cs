using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainShell.Tests.Domain.Models;
using DomainShell.Tests.Domain.Service;

namespace DomainShell.Tests.Web.Controllers
{
    public class PersonController : Controller
    {       
        private PersonReader _personReader = new PersonReader();
        private PersonBulkUpdate _bulkUpdate = new PersonBulkUpdate();
        
        public ActionResult Index()
        {
            return List();
        }
        
        public ActionResult List()
        {
            Person[] persons = _personReader.GetAll();

            return View("List", persons);
        }

        public ActionResult New()
        {
            Person person = new Person();

            return View("Detail", person);
        }        

        public ActionResult Detail(string id)
        {
            Person person = _personReader.Get(id);

            return View(person);
        }

        public ActionResult Bulk()
        {
            Person[] persons = _personReader.GetAll();

            return View(persons);
        }

        public ActionResult Add(Person person)
        {
            bool result = person.Add();

            return Json(result);
        }

        public ActionResult Update(Person person)
        {
            bool result = person.Update();

            return Json(result);
        }

        public ActionResult Remove(Person person)
        {
            bool result = person.Remove();

            return Json(result);
        }

        public ActionResult BulkUpdate(string[] ids, string name)
        {
            PersonBulkUpdate.Result result = _bulkUpdate.BulkUpdate(ids, name);

            return Json(result);
        }
    }
}