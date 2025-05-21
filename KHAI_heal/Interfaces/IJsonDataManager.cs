using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KHAI_heal.Models;
using System.Collections.Generic;

namespace KHAI_heal.Interfaces
{
    public interface IJsonDataManager
    {
        List<User> LoadUsers();
        void SaveUsers(List<User> users);
        List<Appointment> LoadAppointments();
        void SaveAppointments(List<Appointment> appointments);
    }
}