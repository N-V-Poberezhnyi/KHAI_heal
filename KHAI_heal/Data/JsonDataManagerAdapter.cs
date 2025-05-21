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
            throw new NotImplementedException();
        }

        public void SaveUsers(List<User> users)
        {
            throw new NotImplementedException();
        }

        public List<Appointment> LoadAppointments()
        {
            throw new NotImplementedException();
        }

        public void SaveAppointments(List<Appointment> appointments)
        {
            throw new NotImplementedException();
        }
    }
}