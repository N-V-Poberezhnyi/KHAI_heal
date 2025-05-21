using KHAI_heal.Models;
using KHAI_heal.Enums;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System;

namespace KHAI_heal.Tests.Models
{
    public class UserTests
    {
        private Patient CreateValidPatient(int id = 1, string email = "patient@test.com", string password = "password",
            string firstName = "ValidFirstName", string lastName = "ValidLastName", string middleName = "ValidMiddleName")
        {
            return new Patient(id, email, password, firstName, lastName, middleName);
        }

        [Theory]
        [InlineData(null, "password", "Name", "Surname", "Middle")]
        [InlineData("", "password", "Name", "Surname", "Middle")]
        [InlineData("test@test.com", null, "Name", "Surname", "Middle")]
        [InlineData("test@test.com", "", "Name", "Surname", "Middle")]
        [InlineData("test@test.com", "password", null, "Surname", "Middle")]
        [InlineData("test@test.com", "password", "A", "Surname", "Middle")]
        [InlineData("test@test.com", "password", "Name", null, "Middle")]
        public void UserIsValid_WithInvalidData(string email, string password, string firstName, string lastName, string middleName)
        {
            // Arrange
            var user = new Patient(1, email, password, firstName, lastName, middleName);

            // Act
            bool isValid = user.IsValid();

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void UserIsValid()
        {
            // Arrange
            var user = CreateValidPatient();

            // Act
            bool isValid = user.IsValid();

            // Assert
            Assert.True(isValid);
        }


        [Fact]
        public void UpdateProfile_WithValidData()
        {
            // Arrange
            var user = CreateValidPatient(firstName: "Old", lastName: "Old", middleName: "Old");

            // Act
            bool result = user.UpdateBaseProfile("NewFirstName", "NewLastName", "NewMiddleName");

            // Assert
            Assert.True(result);
            Assert.Equal("NewFirstName", user.FirstName);
            Assert.Equal("NewLastName", user.LastName);
            Assert.Equal("NewMiddleName", user.MiddleName);
        }

        [Theory]
        [InlineData("A", "Valid", "Valid")]
        [InlineData("Valid", "B", "Valid")]
        [InlineData("Valid", "Valid", "C")]
        [InlineData("", "Valid", "Valid")]
        [InlineData(null, "Valid", "Valid")]
        public void UpdateProfile_WithInvalidData(string newFirstName, string newLastName, string newMiddleName)
        {
            // Arrange
            var user = CreateValidPatient(firstName: "Old", lastName: "Old", middleName: "Old");

            // Act
            bool result = user.UpdateBaseProfile(newFirstName, newLastName, newMiddleName);

            // Assert
            Assert.False(result);
            Assert.Equal("Old", user.FirstName);
            Assert.Equal("Old", user.LastName);
            Assert.Equal("Old", user.MiddleName);
        }
    }
}