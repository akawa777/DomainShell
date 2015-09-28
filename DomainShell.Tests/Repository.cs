using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Infrastructure;

namespace DomainDesigner.Tests.DomainShell
{ 
    public class PersonData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public static class Persons
    {
        static Persons()
        {
            Persons.persons.Add(new PersonData { Id = 1, Name = "1" });
            Persons.persons.Add(new PersonData { Id = 2, Name = "2" });
            Persons.persons.Add(new PersonData { Id = 3, Name = "3" });
        }

        private static List<PersonData> persons = new List<PersonData>();

        public static List<PersonData> GetAll()
        {
            return persons;
        }

        public static int Insert(PersonData person)
        {
            if (persons.Any(x => x.Id == person.Id))
            {
                return 0;
            }

            persons.Add(person);

            return 1;
        }

        public static int Delete(PersonData person)
        {
            return persons.Remove(person) ? 1 : 0;
        }        
    }

    public class PersonReadRepository
    {
        public Person Load(int id)
        {
            PersonData personData = Persons.GetAll().FirstOrDefault(x => x.Id == id);

            if (personData == null)
            {
                return null;
            }

            Person person = new Person();

            person.Id = personData.Id;
            person.Name = personData.Name;

            return person;
        }

        public int GetNewId()
        {
            if (Persons.GetAll().Count == 0)
            {
                return 1;
            }

            return Persons.GetAll().Max(x => x.Id) + 1;
        }
    }

    public class PersonWriteRepository
    {
        public void Add(Person person)
        {
            PersonData personData = new PersonData();

            personData.Id = person.Id;
            personData.Name = person.Name;

            int rtn = Persons.Insert(personData);

            if (rtn == 0)
            {
                throw new Exception("concurrency exception");
            }
        }

        public void Update(Person person)
        {
            PersonData personData = Persons.GetAll().FirstOrDefault(x => x.Id == person.Id);

            if (personData == null)
            {
                throw new Exception("not exist person");
            }

            personData.Name = person.Name;
        }

        public void Delete(Person person)
        {
            PersonData personData = Persons.GetAll().FirstOrDefault(x => x.Id == person.Id);

            if (personData == null)
            {
                throw new Exception("not exist person");
            }

            int rtn = Persons.Delete(personData);

            if (rtn == 0)
            {
                throw new Exception("concurrency exception");
            }
        }
    }
}
