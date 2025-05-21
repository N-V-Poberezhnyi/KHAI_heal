using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

using KHAI_heal.Models;

namespace KHAI_heal.Data
{
    public static class JsonManager
    {
        private const string UsersFileName = "users.json";
        private const string AppointmentsFileName = "appointments.json";
        private const string DataFolderName = "Data";

        public static string UsersFileNameOverride { get; set; } = null;
        public static string AppointmentsFileNameOverride { get; set; } = null;

        private static string GetDataFolderPath()
        {
            throw new NotImplementedException();
        }

        private static string GetActualUsersFilePath()
        {
            throw new NotImplementedException();
        }

        private static string GetActualAppointmentsFilePath()
        {
            throw new NotImplementedException();
        }

        public static void SaveUsers(List<User> users)
        {
            throw new NotImplementedException();
        }

        public static List<User> LoadUsers()
        {
            throw new NotImplementedException();
        }

        public static void SaveAppointments(List<Appointment> appointments)
        {
            throw new NotImplementedException();
        }

        public static List<Appointment> LoadAppointments()
        {
            throw new NotImplementedException();
        }
    }
}