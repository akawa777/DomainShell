using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainShell.Tests.Domain.Models;
using DomainShell.Tests.Domain.Service;
using System.IO;

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
            return View("List");
        }        

        public ActionResult New()
        {
            return View("Detail");
        }        

        public ActionResult Detail()
        {
            return View();
        }

        public ActionResult Bulk()
        {
            return View();
        }

        public JsonResult GetAll()
        {
            Person[] persons = _personReader.GetAll();

            return Json(persons, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get(string id)
        {
            Person person = _personReader.Get(id);

            return Json(person, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Add(Person person)
        {
            bool result = person.Add();

            return Json(result);
        }

        public JsonResult Update(Person person)
        {
            bool result = person.Update();

            return Json(result);
        }

        public JsonResult Remove(Person person)
        {
            bool result = person.Remove();

            return Json(result);
        }

        public JsonResult BulkUpdate(string[] ids, string name)
        {
            PersonBulkUpdate.Result result = _bulkUpdate.BulkUpdate(ids, name);

            return Json(result);
        }

        public FileResult Output()
        {            
            using (MemoryStream stream = new MemoryStream())
            {                
                _personReader.OutputTsv(stream);    
                return File(stream.GetBuffer(), System.Net.Mime.MediaTypeNames.Application.Octet, "person.txt");
            }
        }
    }
}