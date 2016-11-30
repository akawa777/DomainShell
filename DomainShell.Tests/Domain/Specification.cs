using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain;
using DomainShell.Tests.Domain.Contracts;

namespace DomainShell.Tests.Domain
{
    public class PersonOpions
    {
        public string PersonId { get; set;}
    }

    public class PersondCreationSpec : ICreationSpec<PersonEntity, PersonOpions>
    {
        public PersondCreationSpec()
        {
            
        }

        public PersonOpions Options()
        {
            return new PersonOpions
            {
                
            };
        }

        public void Satisfied(PersonEntity target)
        {
            
        }
    }

    public enum PersonPredicateItem
    {
        LikeName,
        City
    }

    public class PersonPredicate : Dictionary<PersonPredicateItem, object>
    {
        public bool And { get; set; }
    }

    public class PersonLikeNameSelectionSpec : ISelectionSpec<PersonPredicate>
    {
        public PersonLikeNameSelectionSpec(string name)
        {
            _name = name;
        }

        private string _name;

        public PersonPredicate Predicate()
        {
            PersonPredicate predicate = new PersonPredicate();
            predicate.And = true;
            predicate[PersonPredicateItem.LikeName] = _name;

            return predicate;
        }
    }

    public class PersonValidationSpec : IValidationSpec<PersonEntity>
    {
        public bool Validate(PersonEntity target, out string[] errors)
        {
            errors = new string[0];

            return true;
        }
    }
}
