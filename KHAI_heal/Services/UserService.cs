using System.Collections.Generic;
using System.Linq;
using System;

using KHAI_heal.Enums;
using KHAI_heal.Models;
using KHAI_heal.Data;
using KHAI_heal.Interfaces;

namespace KHAI_heal.Services
{
    public class UserService : IUserService
    {
        private List<User> _users;
        private int _nextUserId;

        private readonly IJsonDataManager _jsonDataManager;
        public UserService(IJsonDataManager jsonDataManager)
        {
            throw new NotImplementedException();
        }

        private int GetNextUserId() => throw new NotImplementedException();

        private void SaveUsers() => throw new NotImplementedException();

        public void SaveUser(User user)
        {
            throw new NotImplementedException();
        }

        public User RegisterUser(string email, string password, string firstName, string lastName, string middleName, UserRole role)
        {
            throw new NotImplementedException();
        }

        public User AuthenticateUser(string email, string password)
        {
            throw new NotImplementedException();
        }

        public User GetUserById(int userId) => _users.FirstOrDefault(u => u.Id == userId);

        public bool UpdateUserProfile(int userId, string firstName, string lastName, string middleName)
        {
            throw new NotImplementedException();
        }

        public bool UpdateDoctorProfile(int doctorId, Specialization specialization, decimal price, int experience, List<TimeSpan> schedule)
        {
            throw new NotImplementedException();
        }

        public bool UpdateDoctorPublishedStatus(int doctorId, bool isPublished)
        {
            throw new NotImplementedException();
        }
        public bool GetPublishedStatus(int doctorId)
        {
            throw new NotImplementedException();
        }

        public List<Doctor> GetAllDoctors()
        {
            throw new NotImplementedException();
        }

        public List<Doctor> FindDoctors(string query)
        {
            throw new NotImplementedException();
        }

        public List<Doctor> GetPublishedDoctors()
        {
            throw new NotImplementedException();
        }

        public bool CheckIfEmailExists(string email)
        {
            throw new NotImplementedException();
        }
    }
}
