using System.Windows;
using System.Windows.Controls;
using KHAI_heal.Interfaces;
using KHAI_heal.Models;
using KHAI_heal.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;


namespace KHAI_heal.Views
{
    public partial class UpdateProfileDoctorWindow : Window
    {
        private readonly Doctor _currentDoctor;
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;

        public UpdateProfileDoctorWindow(Doctor currentDoctor, IUserService userService, IAppointmentService appointmentService)
        {
            InitializeComponent();

            _currentDoctor = currentDoctor ?? throw new ArgumentNullException(nameof(currentDoctor));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));

            SpecializationComboBox.ItemsSource = Enum.GetValues(typeof(Specialization)).Cast<Specialization>().ToList();

            LoadDoctorData();
        }

        private void LoadDoctorData()
        {
            LastNameTextBox.Text = _currentDoctor.LastName;
            FirstNameTextBox.Text = _currentDoctor.FirstName;
            MiddleNameTextBox.Text = _currentDoctor.MiddleName;

            if (_currentDoctor.Specialization != default(Specialization))
            {
                SpecializationComboBox.SelectedItem = _currentDoctor.Specialization;
            }

            if (_currentDoctor.Experience > 0)
            {
                ExperienceTextBox.Text = _currentDoctor.Experience.ToString();
            }

            if (_currentDoctor.Price > 0)
            {
                PriceTextBox.Text = _currentDoctor.Price.ToString();
            }

            if (_currentDoctor.Schedule != null && _currentDoctor.Schedule.Any())
            {
                ScheduleTextBox.Text = string.Join(",", _currentDoctor.Schedule.Select(ts => ts.ToString(@"hh\:mm")));
            }
        }

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Text = "";

            string lastName = LastNameTextBox.Text.Trim();
            string firstName = FirstNameTextBox.Text.Trim();
            string middleName = MiddleNameTextBox.Text.Trim();

            Specialization selectedSpecialization = default(Specialization);
            if (SpecializationComboBox.SelectedItem is Specialization spec)
            {
                selectedSpecialization = spec;
            }
            else
            {
                ErrorTextBlock.Text = "Будь ласка, виберіть спеціалізацію.";
                return;
            }

            if (!int.TryParse(ExperienceTextBox.Text.Trim(), out int experience) || experience < 0)
            {
                ErrorTextBlock.Text = "Будь ласка, введіть коректний стаж (ціле невід'ємне число).";
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price <= 0)
            {
                ErrorTextBlock.Text = "Будь ласка, введіть коректну ціну (позитивне число).";
                return;
            }

            List<TimeSpan> schedule = new List<TimeSpan>();
            string[] timeSlots = ScheduleTextBox.Text.Trim().Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var slot in timeSlots)
            {
                if (TimeSpan.TryParseExact(slot, @"h\:mm", CultureInfo.InvariantCulture, out TimeSpan timeSpan) ||
                    TimeSpan.TryParseExact(slot, @"hh\:mm", CultureInfo.InvariantCulture, out timeSpan))
                {
                    schedule.Add(timeSpan);
                }
                else
                {
                    ErrorTextBlock.Text = $"Некоректний формат часу: {slot}. Використовуйте формат HH:mm (наприклад, 09:00, 14:30).";
                    return;
                }
            }
            if (!schedule.Any())
            {
                ErrorTextBlock.Text = "Будь ласка, введіть хоча б один слот у графіку прийому.";
                return;
            }
            schedule = schedule.OrderBy(ts => ts).ToList();

            bool baseUpdateSuccess = _userService.UpdateUserProfile(_currentDoctor.Id, firstName, lastName, middleName);
            bool doctorUpdateSuccess = _userService.UpdateDoctorProfile(_currentDoctor.Id, selectedSpecialization, price, experience, schedule);

            if (baseUpdateSuccess && doctorUpdateSuccess)
            {
                MessageBox.Show("Профіль лікаря успішно оновлено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                _currentDoctor.LastName = lastName;
                _currentDoctor.FirstName = firstName;
                _currentDoctor.MiddleName = middleName;
                _currentDoctor.Specialization = selectedSpecialization;
                _currentDoctor.Experience = experience;
                _currentDoctor.Price = price;
                _currentDoctor.Schedule = schedule;

                this.Close();

            }
            else
            {
                ErrorTextBlock.Text = "Помилка при оновленні профілю. Перевірте дані та спробуйте ще раз.";
            }
        }
    }
}
