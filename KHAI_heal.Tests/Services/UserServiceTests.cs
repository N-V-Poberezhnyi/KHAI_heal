using KHAI_heal.Models;
using KHAI_heal.Enums;
using KHAI_heal.Services;
using KHAI_heal.Interfaces;
using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System;

namespace KHAI_heal.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IJsonDataManager> _mockJsonDataManager;

        public UserServiceTests()
        {
            _mockJsonDataManager = new Mock<IJsonDataManager>();
        }

        private Patient CreateTestPatient(int id = 1, string email = "patient@test.com", string password = "password",
            string firstName = "Patient", string lastName = "Test", string middleName = "Middle")
        {
            return new Patient(id, email, password, firstName, lastName, middleName);
        }

        private Doctor CreateTestDoctor(int id = 10, string email = "doctor@test.com", string password = "password", 
            string firstName = "Doctor", string lastName = "Test", string middleName = "Middle", Specialization specialization = Specialization.Dentist, 
            decimal price = 500, int experience = 5, List<TimeSpan> schedule = null, bool isPublished = true)
        {
            var doctor = new Doctor(
                id, email, password, firstName, lastName, middleName,
                specialization, price, experience,
                schedule ?? new List<TimeSpan> { new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0) }
            );
            doctor.IsPublished = isPublished;
            return doctor;
        }

        private UserService SetUserService(List<User> users)
        {
            _mockJsonDataManager.Reset();
            _mockJsonDataManager.Setup(m => m.LoadUsers()).Returns(users ?? new List<User>());
            return new UserService(_mockJsonDataManager.Object);
        }

        private void VerifySaveNotCalled() =>
            _mockJsonDataManager.Verify(m => m.SaveUsers(It.IsAny<List<User>>()), Times.Never());

        private void VerifySaveUsersOnceWith(string expectedEmail)
        {
            _mockJsonDataManager.Verify(m => m.SaveUsers(
                It.Is<List<User>>(list =>
                    list != null &&
                    list.Count == 1 &&
                    list.Any(u => u.Email == expectedEmail)
                )), Times.Once());
        }


        [Fact]
        public void RegisterUser_WithValidPatientData()
        {
            // Arrange
            var userService = SetUserService(new List<User>());

            // Act
            var registeredUser = userService.RegisterUser("newpatient@test.com", "validPass1", "New", "Patient", "Middle", UserRole.Patient);

            // Assert
            Assert.NotNull(registeredUser);
            Assert.IsType<Patient>(registeredUser);
            Assert.Equal("newpatient@test.com", registeredUser.Email);
            Assert.Equal(UserRole.Patient, registeredUser.Role);
            Assert.True(registeredUser.Id > 0);
            VerifySaveUsersOnceWith("newpatient@test.com");
        }

        [Fact]
        public void RegisterUser_WithExistingEmail()
        {
            // Arrange
            var existingUser = CreateTestPatient(email: "existing@test.com");
            var userService = SetUserService(new List<User> { existingUser });

            // Act
            var registeredUser = userService.RegisterUser("existing@test.com", "validPass1", "New", "Patient", "Middle", UserRole.Patient);

            // Assert
            Assert.Null(registeredUser);
            VerifySaveNotCalled();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("toolongemailtoolongemailtoolongemail@example.com")]
        [InlineData("noatall.com")]
        [InlineData("invalid-email")]
        public void RegisterUser_WithInvalidEmail(string invalidEmail)
        {
            // Arrange
            var userService = SetUserService(new List<User>());

            // Act
            var registeredUser = userService.RegisterUser(invalidEmail, "validPass1", "New", "Patient", "Middle", UserRole.Patient);

            // Assert
            Assert.Null(registeredUser);
            VerifySaveNotCalled();
        }

        [Theory]
        [InlineData("auth@test.com", "validPassword", true)]
        [InlineData("auth@test.com", "wrongPassword", false)]
        [InlineData("wrong@test.com", "validPassword", false)]
        [InlineData("Auth@Test.Com", "validPassword", true)]
        public void AuthenticateUser(string email, string password, bool shouldAuthenticate)
        {
            // Arrange
            var user = CreateTestPatient(id: 1, email: "auth@test.com", password: "validPassword");
            var userService = SetUserService(new List<User> { user });

            // Act
            var authenticatedUser = userService.AuthenticateUser(email, password);

            // Assert
            if (shouldAuthenticate)
            {
                Assert.NotNull(authenticatedUser);
                Assert.Equal(user.Id, authenticatedUser.Id);
            }
            else
            {
                Assert.Null(authenticatedUser);
            }

            VerifySaveNotCalled();
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(10, true)]
        [InlineData(999, false)]
        public void GetUserById(int id, bool shouldFind)
        {
            // Arrange
            var users = new List<User>
            {
                CreateTestPatient(id: 1),
                CreateTestDoctor(id: 10)
            };
            var userService = SetUserService(users);

            // Act
            var result = userService.GetUserById(id);

            // Assert
            if (shouldFind)
            {
                Assert.NotNull(result);
                Assert.Equal(id, result.Id);
            }
            else
            {
                Assert.Null(result);
            }

            VerifySaveNotCalled();
        }


        // Тести для методу UpdateUserProfile()

        [Fact]
        public void UpdateUserProfile_WithValidData()
        {
            // Arrange
            var userToUpdate = CreateTestPatient(id: 1, firstName: "OldName", lastName: "OldSurname", middleName: "OldMiddle");
            var initialUsersData = new List<User> { userToUpdate };
            var userService = SetUserService(initialUsersData);

            string newFirstName = "NewName";
            string newLastName = "NewSurname";
            string newMiddleName = "NewMiddle";

            // Act
            bool result = userService.UpdateUserProfile(1, newFirstName, newLastName, newMiddleName);

            // Assert
            Assert.True(result);
            Assert.Equal(newFirstName, userToUpdate.FirstName);
            Assert.Equal(newLastName, userToUpdate.LastName);
            Assert.Equal(newMiddleName, userToUpdate.MiddleName);
            _mockJsonDataManager.Verify(m => m.SaveUsers(
                It.Is<List<User>>(list =>
                   list != null &&
                   list.Count == 1 &&
                   list.First().Id == 1 &&
                   list.First().FirstName == newFirstName
                )),
                Times.Once());
        }

        [Fact]
        public void UpdateUserProfile_WithNonExistingUser()
        {
            // Arrange
            var userToUpdate = CreateTestPatient(id: 1);
            var initialUsersData = new List<User> { userToUpdate };
            var userService = SetUserService(initialUsersData);

            // Act
            bool result = userService.UpdateUserProfile(999, "New", "New", "New");

            // Assert
            Assert.False(result);
            VerifySaveNotCalled();
        }

        [Theory]
        [InlineData("A", "Valid", "Valid")]
        [InlineData("Valid", "B", "Valid")]
        [InlineData("Valid", "Valid", "C")]
        [InlineData("", "Valid", "Valid")]
        [InlineData(null, "Valid", "Valid")]
        public void UpdateUserProfile_WithInvalidData(string invalidFirstName, string invalidLastName, string invalidMiddleName)
        {
            // Arrange
            var userToUpdate = CreateTestPatient(id: 1, firstName: "OldName", lastName: "OldSurname", middleName: "OldMiddle");
            var initialUsersData = new List<User> { userToUpdate };
            var userService = SetUserService(initialUsersData);

            // Act
            bool result = userService.UpdateUserProfile(1, invalidFirstName, invalidLastName, invalidMiddleName);

            // Assert
            Assert.False(result);
            Assert.Equal("OldName", userToUpdate.FirstName);
            Assert.Equal("OldSurname", userToUpdate.LastName);
            Assert.Equal("OldMiddle", userToUpdate.MiddleName);
            VerifySaveNotCalled();
        }

        [Fact]
        public void UpdateDoctorProfile_WithValidData()
        {
            // Arrange
            var doctorToUpdate = CreateTestDoctor(id: 10, specialization: Specialization.Therapist, price: 500, experience: 5, schedule: new List<TimeSpan>());
            var initialUsersData = new List<User> { doctorToUpdate };
            var userService = SetUserService(initialUsersData);

            Specialization newSpecialization = Specialization.Cardiologist;
            decimal newPrice = 1000;
            int newExperience = 10;
            List<TimeSpan> newSchedule = new List<TimeSpan> { new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0) };

            // Act
            bool result = userService.UpdateDoctorProfile(10, newSpecialization, newPrice, newExperience, newSchedule);

            // Assert
            Assert.True(result);
            Assert.Equal(newSpecialization, doctorToUpdate.Specialization);
            Assert.Equal(newPrice, doctorToUpdate.Price);
            Assert.Equal(newExperience, doctorToUpdate.Experience);
            Assert.Equal(newSchedule, doctorToUpdate.Schedule);
            _mockJsonDataManager.Verify(m => m.SaveUsers(
                It.Is<List<User>>(list => list != null && list.Count == 1 && list.First().Id == 10 &&
                   ((Doctor)list.First()).Specialization == newSpecialization)), Times.Once());
        }

        [Fact]
        public void UpdateDoctorProfile_WithNonExistingDoctor()
        {
            // Arrange
            var doctorToUpdate = CreateTestDoctor(id: 10);
            var initialUsersData = new List<User> { doctorToUpdate };
            var userService = SetUserService(initialUsersData);

            // Act
            bool result = userService.UpdateDoctorProfile(999, Specialization.Cardiologist, 1000, 10, new List<TimeSpan>()); // ID, якого немає

            // Assert
            Assert.False(result);
            VerifySaveNotCalled();
        }

        [Theory]
        [InlineData(Specialization.Cardiologist, 0, 10, new[] { "10:00" })]
        [InlineData(Specialization.Cardiologist, 1000, -5, new[] { "10:00" })]
        [InlineData(Specialization.Cardiologist, 1000, 10, null)]
        [InlineData(Specialization.Cardiologist, 1000, 10, new string[0])]
        public void UpdateDoctorProfile_WithInvalidData(Specialization specialization, decimal price, int experience, string[] scheduleStrings)
        {
            // Arrange
            var doctorToUpdate = CreateTestDoctor(id: 10, specialization: Specialization.Therapist, price: 500, experience: 5, schedule: new List<TimeSpan> { new TimeSpan(9, 0, 0) });
            var initialUsersData = new List<User> { doctorToUpdate };
            var userService = SetUserService(initialUsersData);
            var schedule = scheduleStrings?.Select(TimeSpan.Parse).ToList();

            // Act
            bool result = userService.UpdateDoctorProfile(10, specialization, price, experience, schedule);

            // Assert
            Assert.False(result);
            Assert.Equal(500, doctorToUpdate.Price);
            Assert.Equal(5, doctorToUpdate.Experience);
            Assert.True(doctorToUpdate.Schedule.SequenceEqual(new List<TimeSpan> { new TimeSpan(9, 0, 0) }));


            VerifySaveNotCalled();
        }

        [Theory]
        [InlineData("smith", 2)]
        [InlineData("Cardiologist", 2)]
        [InlineData("NonExistent", 0)]
        [InlineData("Jones", 1)]
        [InlineData("Neurologist", 1)]
        public void FindDoctors(string query, int expectedCount)
        {
            // Arrange
            var initialUsersData = new List<User>
             {
                 CreateTestPatient(id: 1),
                 CreateTestDoctor(id: 10, lastName: "Smith", specialization: Specialization.Cardiologist, isPublished: true),
                 CreateTestDoctor(id: 20, lastName: "Jones", specialization: Specialization.Neurologist, isPublished: true),
                 CreateTestDoctor(id: 30, lastName: "Smithsonian", specialization: Specialization.Cardiologist, isPublished: true)
            };

            var userService = SetUserService(initialUsersData);

            // Act
            var result = userService.FindDoctors(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count);
            Assert.All(result, d => Assert.IsType<Doctor>(d));

            VerifySaveNotCalled();
        }

        [Theory]
        [InlineData("test@example.com", true)]
        [InlineData("TEST@EXAMPLE.COM", true)]
        [InlineData("nonexistent@example.com", false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void CheckIfEmailExists(string email, bool expectedResult)
        {
            // Arrange
            var existingUser = CreateTestPatient(email: "test@example.com");
            var initialUsersData = new List<User> { existingUser };
            var userService = SetUserService(initialUsersData);

            // Act
            bool exists = userService.CheckIfEmailExists(email);

            // Assert
            Assert.Equal(expectedResult, exists);

            VerifySaveNotCalled();
        }

        [Fact]
        public void SaveUser_WithExistingUser()
        {
            // Arrange
            var userToSave = CreateTestPatient(id: 1, firstName: "Initial");
            var initialUsersData = new List<User> { userToSave }; 
            var userService = SetUserService(initialUsersData);
            var userInService = userService.GetUserById(1);
            Assert.NotNull(userInService);
            userInService.FirstName = "Modified";


            // Act
            userService.SaveUser(userInService);

            // Assert
            VerifySaveUsersOnceWith("patient@test.com");
        }

        [Fact]
        public void SaveUser_WithNonExistingUser()
        {
            // Arrange
            var existingUser = CreateTestPatient(id: 1);
            var initialUsersData = new List<User> { existingUser };
            var userService = SetUserService(initialUsersData);
            var nonExistingUser = CreateTestPatient(id: 999);

            // Act
            userService.SaveUser(nonExistingUser);

            // Assert
            VerifySaveNotCalled();
        }
    }
}