﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainShell.Tests.Web.Models;
using DomainShell.Tests.Web.Services;

namespace DomainShell.Tests.Web.Controllers
{
    public class PersonController : Controller
    {
        private PersonReader _personReader = new PersonReader();

        // GET: Person
        public ActionResult Index()
        {
            Person[] persons = _personReader.GetAll();

            return View(persons);
        }

        public ActionResult New()
        {
            Person person = new Person();

            return View("Detail", person);
        }

        public ActionResult Add(Person person)
        {
            bool result = person.Add();

            return Json(result);
        }

        public ActionResult Detail (string id)
        {
            Person person = _personReader.Get(id);

            return View(person);
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
    }
}