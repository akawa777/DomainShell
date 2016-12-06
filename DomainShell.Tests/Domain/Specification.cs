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
    public class PersonConstructorParameters
    {
        
    }

    public class PersondCreationSpec : ICreationSpec<PersonEntity, PersonConstructorParameters>
    {
        public PersondCreationSpec()
        {
            
        }

        public PersonConstructorParameters ConstructorParameters()
        {
            return new PersonConstructorParameters
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

    public class PersonValidationSpec : IValidationSpec<PersonEntity, string>
    {
        public bool Validate(PersonEntity target, out string[] errors)
        {
            errors = new string[0];

            return true;
        }
    }
}
