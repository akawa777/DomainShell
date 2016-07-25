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

        public ActionResult GetAll()
        {
            PersonModel[] persons = _personReader.GetAll();

            return Json(persons, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(string id)
        {
            PersonModel person = _personReader.Get(id);

            return Json(person, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(PersonModel person)
        {
            bool result = person.Add();

            return Json(result);
        }

        public ActionResult Update(PersonModel person)
        {
            bool result = person.Update();

            return Json(result);
        }

        public ActionResult Remove(PersonModel person)
        {
            bool result = person.Remove();

            return Json(result);
        }

        public ActionResult BulkUpdate(string[] ids, string name)
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