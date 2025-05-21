using System;
using System.Windows;
using KHAI_heal.Interfaces;
using KHAI_heal.Services;
using KHAI_heal.Data;

namespace KHAI_heal
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Створюємо всі ваші сервіси та їх залежності (реальні реалізації)
            IJsonDataManager jsonDataManager = new JsonDataManagerAdapter(); // Реальна реалізація менеджера даних
            IUserService userService = new UserService(jsonDataManager); // Створюємо UserService з реальною залежністю
            IAppointmentService appointmentService = new AppointmentService(userService, jsonDataManager); // <-- Створюємо AppointmentService

            // Створюємо та показуємо вікно логіну, передаючи йому UserService та AppointmentService
            LoginWindow loginWindow = new LoginWindow(userService, appointmentService); // <-- Передаємо appointmentService
            loginWindow.Show();
        }
    }
}