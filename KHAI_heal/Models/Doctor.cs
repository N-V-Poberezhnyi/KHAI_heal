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
        public List<TimeSpan> Schedule { get; set; } = new List<TimeSpan>();
        public bool IsPublished { get; set; } = false;

        public Doctor() : base()
        {
            Role = UserRole.Doctor;
            IsPublished = false;
        }
        public Doctor(int id, string email, string password, string firstName, string lastName, string middleName, Specialization specialization, decimal price, int experience, List<TimeSpan> schedule)
            : base(id, email, password, firstName, lastName, middleName, UserRole.Doctor)
        {
            Specialization = specialization;
            Price = price;
            Experience = experience;
            Schedule = schedule ?? new List<TimeSpan>();
            IsPublished = false;
        }

        public override bool IsValid()
        {
            if (!base.IsValid())
                return false;

            if (Specialization != default(Specialization) || IsPublished)
            {
                if (Price <= 0)
                    return false;

                if (Experience < 0)
                    return false;

                if (Schedule == null || !Schedule.Any())
                {
                    return false;
                }
            }

            return true;
        }

        public override string DisplayProfileInfo()
        {
            return $"Лікар: {LastName} {FirstName} {MiddleName} ({Specialization}, Стаж: {Experience} років)";
        }
    }
}
