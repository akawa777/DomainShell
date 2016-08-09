//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using DomainShell.Tests.Domain.Apps;
//using System.IO;

//namespace DomainShell.Tests.Web.Controllers
//{
//    public class PersonController : Controller
//    {
//        private PersonApp _app = new PersonApp();

//        public ActionResult GetAll()
//        {
//            PersonData[] persons = _app.GetAll();

//            return Json(persons, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult Get(string id)
//        {
//            PersonData person = _app.Get(id);

//            return Json(person, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult Add(string name)
//        {
//            string id;
//            Result result = _app.Create(name, out id);

//            return Json(result.Success);
//        }

//        public ActionResult Update(string id, string name)
//        {
//            Result result = _app.Update(id, name);

//            return Json(result.Success);
//        }

//        public ActionResult Remove(string id)
//        {
//            Result result = _app.Delete(id);

//            return Json(result.Success);
//        }

//        public ActionResult BulkUpdate(string[] ids, string name)
//        {
//            Result result = _app.BulkUpdate(ids, name);

//            return Json(result);
//        }

//        public FileResult Output()
//        {
//            using (MemoryStream stream = new MemoryStream())
//            {
//                _app.OutputTsv(stream);
//                return File(stream.GetBuffer(), System.Net.Mime.MediaTypeNames.Application.Octet, "person.txt");
//            }
//        }
//    }
//}