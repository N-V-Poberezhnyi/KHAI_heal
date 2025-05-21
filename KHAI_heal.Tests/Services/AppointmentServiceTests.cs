using Xunit;
using Moq;
using KHAI_heal.Services;
using KHAI_heal.Interfaces;
using KHAI_heal.Models;
using KHAI_heal.Enums;
using System.Collections.Generic;
using System;
using System.Linq;

namespace KHAI_heal.Tests.Services
{
    public class AppointmentServiceTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IJsonDataManager> _mockJsonDataManager;
        private readonly AppointmentService _appointmentService;

        public AppointmentServiceTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockJsonDataManager = new Mock<IJsonDataManager>();
            _mockJsonDataManager.Setup(x => x.LoadAppointments()).Returns(new List<Appointment>());

            _appointmentService = new AppointmentService(_mockUserService.Object, _mockJsonDataManager.Object);
        }

        [Fact]
        public void BookAppointment_WithValidData_ReturnAppointment()
        {
            // Arrange
            var doctorId = 1;
            var patientId = 2;
            var appointmentTime = DateTime.Now.AddDays(1);

            var doctor = new Doctor { Id = doctorId, Schedule = new List<TimeSpan> { appointmentTime.TimeOfDay } };
            var patient = new Patient { Id = patientId, AppointmentIds = new List<int>() };

            _mockUserService.Setup(x => x.GetUserById(doctorId)).Returns(doctor);
            _mockUserService.Setup(x => x.GetUserById(patientId)).Returns(patient);

            // Act
            var result = _appointmentService.BookAppointment(doctorId, patientId, appointmentTime);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(doctorId, result.DoctorId);
            Assert.Equal(patientId, result.PatientId);
            Assert.Equal(AppointmentStatus.Planned, result.Status);
            _mockJsonDataManager.Verify(x => x.SaveAppointments(It.IsAny<List<Appointment>>()), Times.Once);
            _mockUserService.Verify(x => x.SaveUser(patient), Times.Once);
        }

        [Fact]
        public void BookAppointment_WithNonExistingDoctorOrPatient_ReturnsNull()
        {
            // Arrange
            var doctorId = 1;
            var patientId = 2;
            var appointmentTime = DateTime.Now.AddDays(1);

            _mockUserService.Setup(x => x.GetUserById(doctorId)).Returns((Doctor)null);
            _mockUserService.Setup(x => x.GetUserById(patientId)).Returns(new Patient());

            // Act
            var result1 = _appointmentService.BookAppointment(doctorId, patientId, appointmentTime);

            _mockUserService.Setup(x => x.GetUserById(doctorId)).Returns(new Doctor());
            _mockUserService.Setup(x => x.GetUserById(patientId)).Returns((Patient)null);

            var result2 = _appointmentService.BookAppointment(doctorId, patientId, appointmentTime);

            // Assert
            Assert.Null(result1);
            Assert.Null(result2);
        }

        [Fact]
        public void BookAppointment_WithPastDateTime_ReturnsNull()
        {
            // Arrange
            var doctorId = 1;
            var patientId = 2;
            var appointmentTime = DateTime.Now.AddDays(-1);

            var doctor = new Doctor { Id = doctorId, Schedule = new List<TimeSpan> { appointmentTime.TimeOfDay } };
            var patient = new Patient { Id = patientId, AppointmentIds = new List<int>() };

            _mockUserService.Setup(x => x.GetUserById(doctorId)).Returns(doctor);
            _mockUserService.Setup(x => x.GetUserById(patientId)).Returns(patient);

            // Act
            var result = _appointmentService.BookAppointment(doctorId, patientId, appointmentTime);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void BookAppointment_WhenPatientAlreadyHasPlannedAppointment_ReturnsNull()
        {
            // Arrange
            var doctorId = 1;
            var patientId = 2;
            var appointmentTime = DateTime.Now.AddDays(1);

            var existingAppointment = new Appointment(1, appointmentTime.AddHours(1), doctorId, patientId)
            {
                Status = AppointmentStatus.Planned
            };

            var doctor = new Doctor { Id = doctorId, Schedule = new List<TimeSpan> { appointmentTime.TimeOfDay } };
            var patient = new Patient { Id = patientId, AppointmentIds = new List<int> { existingAppointment.Id } };

            _mockUserService.Setup(x => x.GetUserById(doctorId)).Returns(doctor);
            _mockUserService.Setup(x => x.GetUserById(patientId)).Returns(patient);
            _mockJsonDataManager.Setup(x => x.LoadAppointments()).Returns(new List<Appointment> { existingAppointment });

            var service = new AppointmentService(_mockUserService.Object, _mockJsonDataManager.Object);

            // Act
            var result = service.BookAppointment(doctorId, patientId, appointmentTime);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void BookAppointment_WhenTimeSlotIsAlreadyBooked_ReturnsNull()
        {
            // Arrange
            var doctorId = 1;
            var patientId1 = 2;
            var patientId2 = 3;
            var appointmentTime = DateTime.Now.AddDays(1);

            var existingAppointment = new Appointment(1, appointmentTime, doctorId, patientId1)
            {
                Status = AppointmentStatus.Planned
            };

            var doctor = new Doctor { Id = doctorId, Schedule = new List<TimeSpan> { appointmentTime.TimeOfDay } };
            var patient = new Patient { Id = patientId2, AppointmentIds = new List<int>() };

            _mockUserService.Setup(x => x.GetUserById(doctorId)).Returns(doctor);
            _mockUserService.Setup(x => x.GetUserById(patientId2)).Returns(patient);
            _mockJsonDataManager.Setup(x => x.LoadAppointments()).Returns(new List<Appointment> { existingAppointment });

            var service = new AppointmentService(_mockUserService.Object, _mockJsonDataManager.Object);

            // Act
            var result = service.BookAppointment(doctorId, patientId2, appointmentTime);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void BookAppointment_WhenTimeNotInDoctorSchedule_ReturnsNull()
        {
            // Arrange
            var doctorId = 1;
            var patientId = 2;
            var appointmentTime = DateTime.Now.AddDays(1);

            var doctor = new Doctor { Id = doctorId, Schedule = new List<TimeSpan> { appointmentTime.AddHours(1).TimeOfDay } };
            var patient = new Patient { Id = patientId, AppointmentIds = new List<int>() };

            _mockUserService.Setup(x => x.GetUserById(doctorId)).Returns(doctor);
            _mockUserService.Setup(x => x.GetUserById(patientId)).Returns(patient);

            // Act
            var result = _appointmentService.BookAppointment(doctorId, patientId, appointmentTime);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CancelAppointment_WithValidData_ReturnsTrue()
        {
            // Arrange
            var appointmentId = 1;
            var doctorId = 1;
            var patientId = 2;

            var appointment = new Appointment(appointmentId, DateTime.Now.AddDays(1), doctorId, patientId)
            {
                Status = AppointmentStatus.Planned
            };
            var patient = new Patient { Id = patientId, AppointmentIds = new List<int> { appointmentId } };

            _mockUserService.Setup(x => x.GetUserById(patientId)).Returns(patient);
            _mockJsonDataManager.Setup(x => x.LoadAppointments()).Returns(new List<Appointment> { appointment });

            var service = new AppointmentService(_mockUserService.Object, _mockJsonDataManager.Object);

            // Act
            var result = service.CancelAppointment(appointmentId, patientId);

            // Assert
            Assert.True(result);
            Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
            Assert.DoesNotContain(appointmentId, patient.AppointmentIds);
            _mockJsonDataManager.Verify(x => x.SaveAppointments(It.IsAny<List<Appointment>>()), Times.Once);
            _mockUserService.Verify(x => x.SaveUser(patient), Times.Once);
        }

        [Fact]
        public void CancelAppointment_WhenAppointmentNotFound_ReturnsFalse()
        {
            // Arrange
            var appointmentId = 1;
            var patientId = 2;

            _mockJsonDataManager.Setup(x => x.LoadAppointments()).Returns(new List<Appointment>());

            // Act
            var result = _appointmentService.CancelAppointment(appointmentId, patientId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CancelAppointment_WhenPatientNotOwner_ReturnsFalse()
        {
            // Arrange
            var appointmentId = 1;
            var doctorId = 1;
            var patientId1 = 2;
            var patientId2 = 3;

            var appointment = new Appointment(appointmentId, DateTime.Now.AddDays(1), doctorId, patientId1);
            var patient = new Patient { Id = patientId2, AppointmentIds = new List<int>() };

            _mockUserService.Setup(x => x.GetUserById(patientId2)).Returns(patient);
            _mockJsonDataManager.Setup(x => x.LoadAppointments()).Returns(new List<Appointment> { appointment });

            // Act
            var result = _appointmentService.CancelAppointment(appointmentId, patientId2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateAppointmentStatus_WithValidData_ReturnsTrue()
        {
            // Arrange
            var appointmentId = 1;
            var doctorId = 1;
            var patientId = 2;

            var appointment = new Appointment(appointmentId, DateTime.Now.AddDays(1), doctorId, patientId)
            {
                Status = AppointmentStatus.Planned
            };
            var doctor = new Doctor { Id = doctorId };

            _mockUserService.Setup(x => x.GetUserById(doctorId)).Returns(doctor);
            _mockJsonDataManager.Setup(x => x.LoadAppointments()).Returns(new List<Appointment> { appointment });

            var service = new AppointmentService(_mockUserService.Object, _mockJsonDataManager.Object);

            // Act
            var result = service.UpdateAppointmentStatus(
                appointmentId, AppointmentStatus.Completed, doctorId);

            // Assert
            Assert.True(result);
            Assert.Equal(AppointmentStatus.Completed, appointment.Status);
            _mockJsonDataManager.Verify(x => x.SaveAppointments(It.IsAny<List<Appointment>>()), Times.Once);
        }

        [Fact]
        public void UpdateAppointmentStatus_WhenAppointmentNotFound_ReturnsFalse()
        {
            // Arrange
            var appointmentId = 1;
            var doctorId = 1;

            _mockJsonDataManager.Setup(x => x.LoadAppointments()).Returns(new List<Appointment>());

            // Act
            var result = _appointmentService.UpdateAppointmentStatus(
                appointmentId, AppointmentStatus.Completed, doctorId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateAppointmentStatus_WhenDoctorNotOwner_ReturnsFalse()
        {
            // Arrange
            var appointmentId = 1;
            var doctorId1 = 1;
            var doctorId2 = 2;
            var patientId = 2;

            var appointment = new Appointment(appointmentId, DateTime.Now.AddDays(1), doctorId1, patientId);
            var doctor = new Doctor { Id = doctorId2 };

            _mockUserService.Setup(x => x.GetUserById(doctorId2)).Returns(doctor);
            _mockJsonDataManager.Setup(x => x.LoadAppointments()).Returns(new List<Appointment> { appointment });

            // Act
            var result = _appointmentService.UpdateAppointmentStatus(
                appointmentId, AppointmentStatus.Completed, doctorId2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateAppointmentStatus_WhenPastAppointmentAndInvalidStatus_ReturnsFalse()
        {
            // Arrange
            var appointmentId = 1;
            var doctorId = 1;
            var patientId = 2;

            var appointment = new Appointment(appointmentId, DateTime.Now.AddDays(-1), doctorId, patientId);
            var doctor = new Doctor { Id = doctorId };

            _mockUserService.Setup(x => x.GetUserById(doctorId)).Returns(doctor);
            _mockJsonDataManager.Setup(x => x.LoadAppointments()).Returns(new List<Appointment> { appointment });

            // Act
            var result1 = _appointmentService.UpdateAppointmentStatus(
                appointmentId, AppointmentStatus.Planned, doctorId);
            var result2 = _appointmentService.UpdateAppointmentStatus(
                appointmentId, AppointmentStatus.Cancelled, doctorId);

            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }

        [Fact]
        public void GetDoctorAppointments_ReturnsOnlyDoctorsAppointments()
        {
            // Arrange
            var doctorId1 = 1;
            var doctorId2 = 2;
            var patientId = 3;

            var appointment1 = new Appointment(1, DateTime.Now.AddDays(1), doctorId1, patientId);
            var appointment2 = new Appointment(2, DateTime.Now.AddDays(2), doctorId2, patientId);

            _mockJsonDataManager.Setup(x => x.LoadAppointments()).Returns(new List<Appointment> { appointment1, appointment2 });

            var service = new AppointmentService(_mockUserService.Object, _mockJsonDataManager.Object);

            // Act
            var result = service.GetDoctorAppointments(doctorId1);

            // Assert
            Assert.Single(result);
            Assert.Equal(doctorId1, result[0].DoctorId);
        }

        [Fact]
        public void GetPatientAppointments_ReturnsOnlyPatientsAppointments()
        {
            // Arrange
            var doctorId = 1;
            var patientId1 = 2;
            var patientId2 = 3;

            var appointment1 = new Appointment(1, DateTime.Now.AddDays(1), doctorId, patientId1);
            var appointment2 = new Appointment(2, DateTime.Now.AddDays(2), doctorId, patientId2);

            var patient = new Patient { Id = patientId1, AppointmentIds = new List<int> { appointment1.Id } };

            _mockUserService.Setup(x => x.GetUserById(patientId1)).Returns(patient);
            _mockJsonDataManager.Setup(x => x.LoadAppointments()).Returns(new List<Appointment> { appointment1, appointment2 });

            var service = new AppointmentService(_mockUserService.Object, _mockJsonDataManager.Object);

            // Act
            var result = service.GetPatientAppointments(patientId1);

            // Assert
            Assert.Single(result);
            Assert.Equal(patientId1, result[0].PatientId);
        }

        [Fact]
        public void GetAvailableTimeSlots_ReturnsOnlyAvailableSlots()
        {
            // Arrange
            var doctorId = 1;
            var date = DateTime.Now.Date.AddDays(1);
            var bookedTime = new TimeSpan(10, 0, 0);
            var availableTime1 = new TimeSpan(9, 0, 0);
            var availableTime2 = new TimeSpan(11, 0, 0);

            var doctor = new Doctor
            {
                Id = doctorId,
                Schedule = new List<TimeSpan> { bookedTime, availableTime1, availableTime2 }
            };

            var bookedAppointment = new Appointment(1, date.Add(bookedTime), doctorId, 2)
            {
                Status = AppointmentStatus.Planned
            };

            _mockUserService.Setup(x => x.GetUserById(doctorId)).Returns(doctor);
            _mockJsonDataManager.Setup(x => x.LoadAppointments()).Returns(new List<Appointment> { bookedAppointment });

            var service = new AppointmentService(_mockUserService.Object, _mockJsonDataManager.Object);

            // Act
            var result = service.GetAvailableTimeSlots(doctorId, date);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(availableTime1, result);
            Assert.Contains(availableTime2, result);
            Assert.DoesNotContain(bookedTime, result);
        }

        [Fact]
        public void GetAvailableTimeSlots_WhenDoctorNotFound_ReturnsEmptyList()
        {
            // Arrange
            var doctorId = 1;
            var date = DateTime.Now.Date;

            _mockUserService.Setup(x => x.GetUserById(doctorId)).Returns((Doctor)null);

            // Act
            var result = _appointmentService.GetAvailableTimeSlots(doctorId, date);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetAvailableTimeSlots_ExcludesPastTimeSlots()
        {
            // Arrange
            var doctorId = 1;
            var date = DateTime.Now.Date;
            var pastTime = DateTime.Now.TimeOfDay.Subtract(TimeSpan.FromHours(1));
            var futureTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(1));

            var doctor = new Doctor
            {
                Id = doctorId,
                Schedule = new List<TimeSpan> { pastTime, futureTime }
            };

            _mockUserService.Setup(x => x.GetUserById(doctorId)).Returns(doctor);
            _mockJsonDataManager.Setup(x => x.LoadAppointments()).Returns(new List<Appointment>());

            // Act
            var result = _appointmentService.GetAvailableTimeSlots(doctorId, date);

            // Assert
            Assert.Single(result);
            Assert.Contains(futureTime, result);
            Assert.DoesNotContain(pastTime, result);
        }
    }
}