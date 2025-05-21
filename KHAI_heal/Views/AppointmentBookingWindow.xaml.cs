using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KHAI_heal.Interfaces;
using KHAI_heal.Models;
using KHAI_heal.Enums;

namespace KHAI_heal.Views
{
    public partial class AppointmentBookingWindow : Window
    {
        private readonly Doctor _selectedDoctor;
        private readonly Patient _currentPatient;
        private readonly IAppointmentService _appointmentService;
        private readonly IUserService _userService;

        public AppointmentBookingWindow(Doctor selectedDoctor, Patient currentPatient, IAppointmentService appointmentService, IUserService userService)
        {
            InitializeComponent();

            _selectedDoctor = selectedDoctor ?? throw new ArgumentNullException(nameof(selectedDoctor));
            _currentPatient = currentPatient ?? throw new ArgumentNullException(nameof(currentPatient));
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            DisplayDoctorInfo();
            LoadAvailableTimeSlots(DateTime.Today);
        }

        private void DisplayDoctorInfo()
        {
            DoctorInfoTextBlock.Text = $"Лікар: {_selectedDoctor.LastName} {_selectedDoctor.FirstName} {_selectedDoctor.MiddleName}";
            DoctorSpecializationTextBlock.Text = $"Спеціалізація: {_selectedDoctor.Specialization}";
            DoctorExperienceTextBlock.Text = $"Стаж: {_selectedDoctor.Experience} років";
        }

        private void LoadAvailableTimeSlots(DateTime date)
        {
            ErrorTextBlock.Text = "";

            List<TimeSpan> availableTimeSpans = _appointmentService.GetAvailableTimeSlots(_selectedDoctor.Id, date);

            if (availableTimeSpans != null && availableTimeSpans.Any())
            {
                List<DateTime> availableDateTimes = availableTimeSpans.Select(ts => date.Date + ts).ToList();

                AvailableTimeSlotsListBox.ItemsSource = availableDateTimes.OrderBy(dt => dt).ToList();
                BookAppointmentButton.IsEnabled = true;
            }
            else
            {
                ErrorTextBlock.Text = $"На жаль, немає доступних годин для запису на {date.ToShortDateString()}.";
                AvailableTimeSlotsListBox.ItemsSource = null;
                BookAppointmentButton.IsEnabled = false;
            }
        }

        private void BookAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Text = "";

            DateTime selectedDateTime = (DateTime)AvailableTimeSlotsListBox.SelectedItem;

            if (selectedDateTime == default)
            {
                ErrorTextBlock.Text = "Будь ласка, виберіть доступний час для запису.";
                return;
            }
            if (_currentPatient == null)
            {
                ErrorTextBlock.Text = "Тільки пацієнти можуть записуватися на прийом.";
                return;
            }

            Appointment newAppointment = _appointmentService.BookAppointment(_selectedDoctor.Id, _currentPatient.Id, selectedDateTime);

            if (newAppointment != null)
            {
                MessageBox.Show("Запис успішно створено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                ErrorTextBlock.Text = "Не вдалося створити запис. Можливо, час вже зайнятий або виникла інша помилка.";
                LoadAvailableTimeSlots(selectedDateTime.Date);
            }
        }
    }
}
