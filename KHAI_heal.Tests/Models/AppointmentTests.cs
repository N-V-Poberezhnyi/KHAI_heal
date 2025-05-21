using KHAI_heal.Models;
using KHAI_heal.Enums;
using Xunit;
using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KHAI_heal.Tests.Models
{
    public class AppointmentTests
    {
        private Appointment CreateAppointment(int id = 1, DateTime? dateTime = null, int doctorId = 10, int patientId = 1, AppointmentStatus status = AppointmentStatus.Planned)
        {
            var appointmentDateTime = dateTime ?? DateTime.Today.AddDays(1).AddHours(10);

            var appointment = new Appointment(id, appointmentDateTime, doctorId, patientId);

            appointment.Status = status;

            return appointment;
        }

        [Fact]
        public void Appointment_Constructor_SettingValues()
        {
            // Arrange
            int id = 101;
            var dateTime = new DateTime(2025, 12, 31, 14, 0, 0);
            int doctorId = 50;
            int patientId = 25;

            // Act
            var appointment = new Appointment(id, dateTime, doctorId, patientId);

            // Assert
            Assert.Equal(id, appointment.Id);
            Assert.Equal(dateTime, appointment.AppointmentDateTime);
            Assert.Equal(doctorId, appointment.DoctorId);
            Assert.Equal(patientId, appointment.PatientId);
            Assert.Equal(AppointmentStatus.Planned, appointment.Status);
        }

        [Theory]
        [InlineData(AppointmentStatus.Planned, AppointmentStatus.Completed, true)]
        [InlineData(AppointmentStatus.Planned, AppointmentStatus.Cancelled, true)]
        [InlineData(AppointmentStatus.Planned, AppointmentStatus.Planned, true)]
        [InlineData(AppointmentStatus.Completed, AppointmentStatus.Planned, true)]
        [InlineData(AppointmentStatus.Completed, AppointmentStatus.Cancelled, true)]
        [InlineData(AppointmentStatus.Completed, AppointmentStatus.Completed, true)]
        [InlineData(AppointmentStatus.Cancelled, AppointmentStatus.Planned, true)]
        [InlineData(AppointmentStatus.Cancelled, AppointmentStatus.Completed, true)]
        [InlineData(AppointmentStatus.Cancelled, AppointmentStatus.Cancelled, true)]
        public void UpdateStatus_ByDoctor(AppointmentStatus initialStatus, AppointmentStatus newStatus, bool isDoctor)
        {
            // Arrange
            var appointment = CreateAppointment(status: initialStatus);

            // Act
            bool result = appointment.UpdateStatus(newStatus, isDoctor);

            // Assert
            Assert.True(result);
            Assert.Equal(newStatus, appointment.Status);
        }

        [Theory]
        [InlineData(AppointmentStatus.Planned, AppointmentStatus.Cancelled, false, true)]
        [InlineData(AppointmentStatus.Planned, AppointmentStatus.Completed, false, false)]
        [InlineData(AppointmentStatus.Planned, AppointmentStatus.Planned, false, false)]
        [InlineData(AppointmentStatus.Completed, AppointmentStatus.Planned, false, false)]
        [InlineData(AppointmentStatus.Completed, AppointmentStatus.Completed, false, false)]
        [InlineData(AppointmentStatus.Completed, AppointmentStatus.Cancelled, false, false)]
        [InlineData(AppointmentStatus.Cancelled, AppointmentStatus.Planned, false, false)]
        [InlineData(AppointmentStatus.Cancelled, AppointmentStatus.Completed, false, false)]
        [InlineData(AppointmentStatus.Cancelled, AppointmentStatus.Cancelled, false, false)]
        public void UpdateStatus_ByPatient(AppointmentStatus initialStatus, AppointmentStatus newStatus, bool isDoctor, bool expectedResult)
        {
            // Arrange
            var appointment = CreateAppointment(status: initialStatus);
            var statusBeforeUpdate = appointment.Status;

            // Act
            bool result = appointment.UpdateStatus(newStatus, isDoctor);

            // Assert:
            Assert.Equal(expectedResult, result);

            if (expectedResult)
            {
                Assert.Equal(newStatus, appointment.Status);
            }
            else
            {
                Assert.Equal(statusBeforeUpdate, appointment.Status);
            }
        }

        [Fact]
        public void IsPastAppointment_ForFutureAppointment()
        {
            // Arrange
            var futureAppointment = CreateAppointment(dateTime: DateTime.Now.AddHours(1));

            // Act + Assert:
            Assert.False(futureAppointment.IsPastAppointment());
        }

        [Fact]
        public void IsPastAppointment_ForPastAppointment()
        {
            // Arrange
            var pastAppointment = CreateAppointment(dateTime: DateTime.Now.AddHours(-1));

            // Act + Assert:
            Assert.True(pastAppointment.IsPastAppointment());
        }
    }
}