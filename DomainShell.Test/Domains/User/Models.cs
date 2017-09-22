using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test.Domains.User
{
    public class UserModel : IAggregateRoot
    {
        protected UserModel()
        {
            
        }

        public string UserId { get; private set; }

        public string UserName { get; set; }

        public int RecordVersion { get; private set; }

        public Dirty Dirty { get; private set; }

        public bool Deleted { get; private set; }
    }

    public class UserValue
    {
        protected UserValue()
        {

        }

        public static UserValue Create(UserModel userModel)
        {
            if (userModel == null || userModel.RecordVersion == 0 || userModel.Dirty.Is()) throw new ArgumentException("userModel is invalid.");

            UserValue userValue = new UserValue();
            userValue.UserId = userModel.UserId;

            return userValue;
        }

        public string UserId { get; private set; }
    }
}