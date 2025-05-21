using KHAI_heal.Models;
using KHAI_heal.Enums;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System;

namespace KHAI_heal.Tests.Models
{
    public class DoctorTests
    {
        private Doctor CreateValidDoctor(int id = 1, string email = "doctor@test.com", string password = "password",
            string firstName = "FirstName", string lastName = "LastName", string middleName = "MiddleName",
            Specialization specialization = Specialization.Dentist, decimal price = 500,
            int experience = 5, List<TimeSpan> schedule = null)
        {
            var validSchedule = schedule ?? new List<TimeSpan> { new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0) };

            return new Doctor(id, email, password, firstName, lastName, middleName,
                specialization, price, experience, validSchedule);
        }

        [Fact]
        public void DoctorIsValid()
        {
            // Arrange
            var doctor = CreateValidDoctor();

            // Act
            bool isValid = doctor.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData(Specialization.Dentist, -100, 5, new[] { "09:00" })]
        [InlineData(Specialization.Dentist, 500, -1, new[] { "09:00" })]
        [InlineData(Specialization.Dentist, 500, 5, null)]
        [InlineData(Specialization.Dentist, 500, 5, new string[0])]
        public void DoctorIsValid_InvalidData(Specialization specialization, decimal price, int experience, string[] scheduleStrings)
        {
            // Arrange
            var schedule = scheduleStrings?.Select(TimeSpan.Parse).ToList();

            var doctor = new Doctor(1, "doctor@test.com", "password", "Name", "Surname", "Middle", specialization, price, experience, schedule);

            // Act
            bool isValid = doctor.IsValid();

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void DoctorIsValid_WithInvalidEmail()
        {
            // Arrange
            var doctor = CreateValidDoctor(email: "invalid-email");

            // Act
            bool isValid = doctor.IsValid();

            // Assert
            Assert.False(isValid);
        }

    }
}