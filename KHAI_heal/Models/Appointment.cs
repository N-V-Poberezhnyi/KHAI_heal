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
        [JsonIgnore]
        public string DoctorName { get; set; }
        [JsonIgnore]
        public string PatientName { get; set; }

        public Appointment() { }
        public Appointment(int id, DateTime appointmentDateTime, int doctorId, int patientId)
        {
            Id = id;
            AppointmentDateTime = appointmentDateTime;
            DoctorId = doctorId;
            PatientId = patientId;
            Status = AppointmentStatus.Planned;
        }

        public bool UpdateStatus(AppointmentStatus newStatus, bool isDoctor)
        {
            if (isDoctor)
            {
                Status = newStatus;
                return true;
            }
            else // Пацієнт
            {
                if (Status == AppointmentStatus.Planned && newStatus == AppointmentStatus.Cancelled)
                {
                    Status = newStatus;
                    return true;
                }
                return false;
            }
        }
        public bool IsPastAppointment()
        {
            return AppointmentDateTime < DateTime.Now;
        }
    }
}

