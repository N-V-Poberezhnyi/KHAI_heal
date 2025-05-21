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
            Id = id;
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            Role = role;
        }

        public virtual bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Email) || Email.Length < 3 || Email.Length > 25 || !Email.Contains("@"))
                return false;

            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 5 || Password.Length > 20)
                return false;

            if (string.IsNullOrWhiteSpace(FirstName) || FirstName.Length < 2) return false;
            if (string.IsNullOrWhiteSpace(LastName) || LastName.Length < 2) return false;
            if (string.IsNullOrWhiteSpace(MiddleName) || MiddleName.Length < 2) return false;

            return true;
        }

        public bool UpdateBaseProfile(string firstName, string lastName, string middleName)
        {
            string oldFirstName = FirstName;
            string oldLastName = LastName;
            string oldMiddleName = MiddleName;

            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;

            if (!IsValid())
            {
                FirstName = oldFirstName;
                LastName = oldLastName;
                MiddleName = oldMiddleName;
                return false; // дані не валідні
            }

            return true;
        }

        public abstract string DisplayProfileInfo();
    }
}
