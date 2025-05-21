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
            string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string executableDirectory = Path.GetDirectoryName(executablePath);

            string dataFolderPath = Path.Combine(executableDirectory, DataFolderName);

            return dataFolderPath;
        }

        private static string GetActualUsersFilePath()
        {
            string dataFolderPath = GetDataFolderPath();
            string fileName = UsersFileNameOverride ?? UsersFileName;
            return Path.Combine(dataFolderPath, fileName);
        }

        private static string GetActualAppointmentsFilePath()
        {
            string dataFolderPath = GetDataFolderPath();

            string fileName = AppointmentsFileNameOverride ?? AppointmentsFileName;
            return Path.Combine(dataFolderPath, fileName);
        }

        public static void SaveUsers(List<User> users)
        {
            string filePath = GetActualUsersFilePath();
            string dataFolderPath = GetDataFolderPath();

            try
            {
                if (!Directory.Exists(dataFolderPath))
                {
                    Directory.CreateDirectory(dataFolderPath);
                }

                string json = JsonConvert.SerializeObject(users, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto // десеріалізація Patient/Doctor
                });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving users to JSON file '{filePath}': {ex.Message}");
            }
        }

        public static List<User> LoadUsers()
        {
            string filePath = GetActualUsersFilePath();

            if (!File.Exists(filePath))
            {
                return new List<User>();
            }

            try
            {
                string json = File.ReadAllText(filePath);
                var users = JsonConvert.DeserializeObject<List<User>>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                return users ?? new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження користувачів із файлу JSON '{filePath}': {ex.Message}");

                return new List<User>();
            }
        }

        public static void SaveAppointments(List<Appointment> appointments)
        {
            string filePath = GetActualAppointmentsFilePath();
            string dataFolderPath = GetDataFolderPath();

            try
            {
                if (!Directory.Exists(dataFolderPath))
                {
                    Directory.CreateDirectory(dataFolderPath);
                }

                string json = JsonConvert.SerializeObject(appointments, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка збереження файлу у JSON '{filePath}': {ex.Message}");
            }
        }

        public static List<Appointment> LoadAppointments()
        {
            string filePath = GetActualAppointmentsFilePath();
            if (!File.Exists(filePath))
            {
                return new List<Appointment>();
            }
            try
            {
                string json = File.ReadAllText(filePath);
                var appointments = JsonConvert.DeserializeObject<List<Appointment>>(json);
                return appointments ?? new List<Appointment>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при завантаженні файлу із JSON '{filePath}': {ex.Message}");
                return new List<Appointment>();
            }
        }
    }
}