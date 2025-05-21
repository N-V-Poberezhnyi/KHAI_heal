using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KHAI_heal.Enums;
using KHAI_heal.Models;

namespace KHAI_heal.Interfaces
{
    public interface IUserService
    {

        User RegisterUser(string email, string password, string firstName, string lastName, string middleName, UserRole role);
        User AuthenticateUser(string email, string password);
        User GetUserById(int userId);
        bool UpdateUserProfile(int userId, string firstName, string lastName, string middleName);
        bool UpdateDoctorProfile(int doctorId, Specialization specialization, decimal price, int experience, List<TimeSpan> schedule);
        bool GetPublishedStatus(int doctorId);
        public bool UpdateDoctorPublishedStatus(int doctorId, bool isPublished);
        List<Doctor> GetAllDoctors();
        List<Doctor> FindDoctors(string query);
        bool CheckIfEmailExists(string email);
        void SaveUser(User user);
    }
}
