using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using KHAI_heal.Enums;

namespace KHAI_heal.Models
{
    public class Patient : User
    {
        public List<int> AppointmentIds { get; set; } = new List<int>();

        public Patient() : base()
        {
            Role = UserRole.Patient;
        }

        public Patient(int id, string email, string password, string firstName, string lastName, string middleName)
            : base(id, email, password, firstName, lastName, middleName, UserRole.Patient)
        {
        }

        public override bool IsValid()
        {
            return base.IsValid();
        }

        public override string DisplayProfileInfo()
        {
            return $"Пацієнт: {LastName} {FirstName} {MiddleName}";
        }
    }
}
