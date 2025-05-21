using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KHAI_heal.Interfaces;
using KHAI_heal.Models;
using System.Collections.Generic;

namespace KHAI_heal.Data
{
    public class JsonDataManagerAdapter : IJsonDataManager
    {
        public List<User> LoadUsers()
        {
            return JsonManager.LoadUsers();
        }

        public void SaveUsers(List<User> users)
        {
            JsonManager.SaveUsers(users);
        }

        public List<Appointment> LoadAppointments()
        {
            return JsonManager.LoadAppointments();
        }

        public void SaveAppointments(List<Appointment> appointments)
        {
            JsonManager.SaveAppointments(appointments);
        }
    }
}