using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KHAI_heal.Enums;
using KHAI_heal.Models;
using KHAI_heal.Interfaces;
using KHAI_heal.Data;
using Newtonsoft.Json;

namespace KHAI_heal.Services
{
    public class AppointmentService : IAppointmentService
    {
        private List<Appointment> _appointments;
        private int _nextAppointmentId;

        private readonly IUserService _userService;
        private readonly IJsonDataManager _jsonDataManager;

        public AppointmentService(IUserService userService, IJsonDataManager jsonDataManager)
        {
            _userService = userService;
            _jsonDataManager = jsonDataManager;

            _appointments = _jsonDataManager.LoadAppointments();
            _nextAppointmentId = _appointments.Any() ? _appointments.Max(a => a.Id) + 1 : 1;
            _jsonDataManager = jsonDataManager;
        }

        private int GetNextAppointmentId() => _nextAppointmentId++;

        private void SaveAppointments() => _jsonDataManager.SaveAppointments(_appointments);

        private void PopulateAppointmentNames(List<Appointment> appointments)
        {
            foreach (var app in appointments)
            {
                var doctor = _userService.GetUserById(app.DoctorId) as Doctor;
                if (doctor != null)
                {
                    app.DoctorName = $"{doctor.LastName} {doctor.FirstName} {doctor.MiddleName}".Trim();
                }
                var patient = _userService.GetUserById(app.PatientId) as Patient;
                if (patient != null)
                {
                    app.PatientName = $"{patient.LastName} {patient.FirstName} {patient.MiddleName}".Trim();
                }
            }
        }

        public Appointment BookAppointment(int doctorId, int patientId, DateTime appointmentDateTime)
        {
            var doctor = _userService.GetUserById(doctorId) as Doctor;
            var patient = _userService.GetUserById(patientId) as Patient;

            if (doctor == null || patient == null) return null;

            if (_appointments.Any(a => a.PatientId == patientId && a.DoctorId == doctorId && a.Status == AppointmentStatus.Planned))
            {
                return null; // Пацієнт вже має запланований запис до цього лікаря
            }

            if (appointmentDateTime < DateTime.Now) return null; // Не можна записатися на минулі сеанси

            if (_appointments.Any(a => a.DoctorId == doctorId && a.AppointmentDateTime == appointmentDateTime && (a.Status == AppointmentStatus.Planned || a.Status == AppointmentStatus.Completed)))
            {
                return null; // Цей час вже зайнятий
            }

            bool isTimeInSchedule = doctor.Schedule != null && doctor.Schedule.Contains(appointmentDateTime.TimeOfDay);

            if (!isTimeInSchedule)
            {
                return null; // Час не входить у графік лікаря
            }

            int newId = GetNextAppointmentId();
            var newAppointment = new Appointment(newId, appointmentDateTime, doctorId, patientId);

            _appointments.Add(newAppointment);
            patient.AppointmentIds.Add(newId);

            SaveAppointments();
            _userService.SaveUser(patient);

            return newAppointment;
        }

        public bool CancelAppointment(int appointmentId, int patientId)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == appointmentId);
            var patient = _userService.GetUserById(patientId) as Patient;

            if (appointment == null || patient == null || appointment.PatientId != patientId)
                return false; // Запис не знайдено, або пацієнт не є власником запису

            bool success = appointment.UpdateStatus(AppointmentStatus.Cancelled, isDoctor: false);

            if (success)
            {
                patient.AppointmentIds.Remove(appointmentId);

                SaveAppointments();
                _userService.SaveUser(patient);
            }

            return success;
        }

        public bool UpdateAppointmentStatus(int appointmentId, AppointmentStatus newStatus, int doctorId)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == appointmentId);
            var doctor = _userService.GetUserById(doctorId) as Doctor;

            if (appointment == null || doctor == null || appointment.DoctorId != doctorId)
                return false; // Запис не знайдено, або лікар не є власником запису

            if (appointment.IsPastAppointment() && (newStatus == AppointmentStatus.Planned || newStatus == AppointmentStatus.Cancelled))
            {
                return false; // Не можна змінити минулі записи
            }

            bool success = appointment.UpdateStatus(newStatus, isDoctor: true);

            if (success)
            {
                SaveAppointments();
            }

            return success;
        }

        public List<Appointment> GetDoctorAppointments(int doctorId)
        {
            var doctorAppointments = _appointments.Where(a => a.DoctorId == doctorId).ToList();
            PopulateAppointmentNames(doctorAppointments);
            return doctorAppointments;
        }

        public List<Appointment> GetPatientAppointments(int patientId)
        {
            var patient = _userService.GetUserById(patientId) as Patient;
            if (patient == null) return new List<Appointment>();

            var patientAppointments = _appointments.Where(a => patient.AppointmentIds.Contains(a.Id)).ToList();

            PopulateAppointmentNames(patientAppointments);
            return patientAppointments;
        }

        public List<TimeSpan> GetAvailableTimeSlots(int doctorId, DateTime date)
        {
            var doctor = _userService.GetUserById(doctorId) as Doctor;

            if (doctor == null) return new List<TimeSpan>(); // Лікар не знайдено

            var bookedSlots = _appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDateTime.Date == date.Date && a.Status != AppointmentStatus.Cancelled)
                .Select(a => a.AppointmentDateTime.TimeOfDay)
                .ToList();

            var availableSlots = doctor.Schedule
                .Where(ts => !bookedSlots.Contains(ts) && new DateTime(date.Year, date.Month, date.Day, ts.Hours, ts.Minutes, ts.Seconds) >= DateTime.Now)
                .ToList();

            availableSlots.Sort();

            return availableSlots;
        }
    }
}
