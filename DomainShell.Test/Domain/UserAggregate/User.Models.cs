using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test.Domain.UserAggregate
{
    public class User : AggregateRoot
    {   
        protected User()
        {
            
        }

        public User Create(string userId)
        {
            var user = DomainModelFactory.Create<User>();

            user.UserId = userId;

            return user;
        }

        public string UserId { get; private set; }

        public string UserName { get; set; }

        public int PaymentPoint { get; set; }

        public void Register()
        {
            if (string.IsNullOrEmpty(UserName)) throw new Exception("UserName is required.");
            if (PaymentPoint < 0) throw new Exception("PaymentPoint is invalid.");

            DomainEvents.Add(new UserRegisterdEvent { UserId = UserId });
            State = ModelState.Seal(this);
        }
    }

    public class UserRegisterdEvent: IDomainEvent
    {
        public string UserId { get; set; }
    }
}