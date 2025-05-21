using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using KHAI_heal.Enums;


namespace KHAI_heal.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }

        public Appointment() { }
        public Appointment(int id, DateTime appointmentDateTime, int doctorId, int patientId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateStatus(AppointmentStatus newStatus, bool isDoctor)
        {
            throw new NotImplementedException();
        }
        public bool IsPastAppointment()
        {
            throw new NotImplementedException();
        }
    }
}
