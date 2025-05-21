using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KHAI_heal.Enums;

namespace KHAI_heal.Models
{
    public class Doctor : User
    {
        public Specialization Specialization { get; set; }
        public decimal Price { get; set; }
        public int Experience { get; set; }
        public List<TimeSpan> Schedule { get; set; }
        public bool IsPublished { get; set; } = false;
       

        public Doctor() : base()
        {
            throw new NotImplementedException();
        }
        public Doctor(int id, string email, string password, string firstName, string lastName, string middleName, Specialization specialization, decimal price, int experience, List<TimeSpan> schedule)
            : base(id, email, password, firstName, lastName, middleName, UserRole.Doctor)
        {
            throw new NotImplementedException();
        }

        public override bool IsValid()
        {
            throw new NotImplementedException();
        }

        public override string DisplayProfileInfo()
        {
            throw new NotImplementedException();
        }
    }
}
