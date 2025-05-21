using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KHAI_heal.Enums;
using KHAI_heal.Models;


namespace KHAI_heal.Interfaces
{
    public interface IAppointmentService
    {
        Appointment BookAppointment(int doctorId, int patientId, DateTime appointmentDateTime);
        bool CancelAppointment(int appointmentId, int patientId);
        bool UpdateAppointmentStatus(int appointmentId, AppointmentStatus newStatus, int doctorId);
        List<Appointment> GetDoctorAppointments(int doctorId);
        List<Appointment> GetPatientAppointments(int patientId);
        List<TimeSpan> GetAvailableTimeSlots(int doctorId, DateTime date);
    }
}
