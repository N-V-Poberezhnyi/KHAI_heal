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
            _jsonDataManager = jsonDataManager;

            _users = _jsonDataManager.LoadUsers();

            _nextUserId = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
        }

        private int GetNextUserId() => _nextUserId++;

        private void SaveUsers() => _jsonDataManager.SaveUsers(_users);

        public void SaveUser(User user)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                SaveUsers();
            }
        }

        public User RegisterUser(string email, string password, string firstName, string lastName, string middleName, UserRole role)
        {
            if (CheckIfEmailExists(email))
            {
                return null; // Користувач з такою поштою вже існує
            }

            int newId = GetNextUserId();
            User newUser;

            if (role == UserRole.Doctor)
            {
                newUser = new Doctor(newId, email, password, firstName, lastName, middleName, default, 0, 0, new List<TimeSpan>());
            }
            else // Пацієнт
            {
                newUser = new Patient(newId, email, password, firstName, lastName, middleName);
            }
            if (!newUser.IsValid())
            {
                return null;
            }


            _users.Add(newUser);
            SaveUsers();
            return newUser;
        }

        public User AuthenticateUser(string email, string password)
        {
            var user = _users.FirstOrDefault(u => u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase) && u.Password == password);

            if (user == null)
            {
                return null; // Невірний логін або пароль
            }

            return user;
        }

        public User GetUserById(int userId) => _users.FirstOrDefault(u => u.Id == userId);

        public bool UpdateUserProfile(int userId, string firstName, string lastName, string middleName)
        {
            var user = GetUserById(userId);
            if (user == null)
                return false;

            if (!user.UpdateBaseProfile(firstName, lastName, middleName))
            {
                return false;
            }

            SaveUsers();
            return true;
        }

        public bool UpdateDoctorProfile(int doctorId, Specialization specialization, decimal price, int experience, List<TimeSpan> schedule)
        {
            var doctor = _users.OfType<Doctor>().FirstOrDefault(d => d.Id == doctorId);
            if (doctor == null)
                return false;

            var oldSpecialization = doctor.Specialization;
            var oldPrice = doctor.Price;
            var oldExperience = doctor.Experience;
            var oldSchedule = doctor.Schedule;

            doctor.Specialization = specialization;
            doctor.Price = price;
            doctor.Experience = experience;
            doctor.Schedule = schedule ?? new List<TimeSpan>();

            if (!doctor.IsValid())
            {
                doctor.Specialization = oldSpecialization;
                doctor.Price = oldPrice;
                doctor.Experience = oldExperience;
                doctor.Schedule = oldSchedule;
                return false; // Помилка валідації
            }

            SaveUsers();
            return true;
        }

        public bool UpdateDoctorPublishedStatus(int doctorId, bool isPublished)
        {
            var doctor = _users.OfType<Doctor>().FirstOrDefault(d => d.Id == doctorId);
            if (doctor == null)
                return false;

            doctor.IsPublished = isPublished;

            if (isPublished && !doctor.IsValid())
            {
                doctor.IsPublished = !isPublished;
                return false;
            }
            SaveUsers();
            return true;
        }
        public bool GetPublishedStatus(int doctorId)
        {
            var doctor = _users.OfType<Doctor>().FirstOrDefault(d => d.Id == doctorId);
            return doctor?.IsPublished ?? false;
        }

        public List<Doctor> GetAllDoctors()
        {
            return _users.OfType<Doctor>().ToList();
        }

        public List<Doctor> FindDoctors(string query)
        {
            List<Doctor> publishedDoctors = GetPublishedDoctors();

            if (string.IsNullOrWhiteSpace(query))
            {
                return publishedDoctors;
            }

            // Пошук за прізвищем або спеціальністю
            string lowerQuery = query.Trim().ToLower();
            return publishedDoctors
                        .Where(d => (d.LastName != null && d.LastName.Trim().ToLower().Contains(lowerQuery)) ||
                                    (d.Specialization.ToString().Trim().ToLower().Contains(lowerQuery))).ToList();
        }

        public List<Doctor> GetPublishedDoctors()
        {
            return _users.OfType<Doctor>().Where(d => d.IsPublished).ToList();
        }

        public bool CheckIfEmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return _users.Any(u => u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
