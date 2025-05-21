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
            throw new NotImplementedException();
        }

        private int GetNextAppointmentId() => _nextAppointmentId++;

        private void SaveAppointments() => _jsonDataManager.SaveAppointments(_appointments);

        private void PopulateAppointmentNames(List<Appointment> appointments)
        {
            throw new NotImplementedException();
        }

        public Appointment BookAppointment(int doctorId, int patientId, DateTime appointmentDateTime)
        {
            throw new NotImplementedException();
        }

        public bool CancelAppointment(int appointmentId, int patientId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAppointmentStatus(int appointmentId, AppointmentStatus newStatus, int doctorId)
        {
            throw new NotImplementedException();
        }

        public List<Appointment> GetDoctorAppointments(int doctorId)
        {
            throw new NotImplementedException();
        }

        public List<Appointment> GetPatientAppointments(int patientId)
        {
            throw new NotImplementedException();
        }

        public List<TimeSpan> GetAvailableTimeSlots(int doctorId, DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
