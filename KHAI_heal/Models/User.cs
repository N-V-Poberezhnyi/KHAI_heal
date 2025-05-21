using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using KHAI_heal.Enums;

namespace KHAI_heal.Models
{
    public abstract class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public UserRole Role { get; set; }
        public User() { }

        protected User(int id, string email, string password, string firstName, string lastName, string middleName, UserRole role)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsValid()
        {
            throw new NotImplementedException();
        }

        public bool UpdateBaseProfile(string firstName, string lastName, string middleName)
        {
            throw new NotImplementedException();
        }

        public abstract string DisplayProfileInfo();
    }
}
