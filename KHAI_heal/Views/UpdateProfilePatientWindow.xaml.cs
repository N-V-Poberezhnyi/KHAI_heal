using System.Windows;
using System.Windows.Controls;
using KHAI_heal.Interfaces;
using KHAI_heal.Models;
using System;

namespace KHAI_heal.Views
{
    public partial class UpdateProfilePatientWindow : Window
    {
        private readonly Patient _currentPatient;
        private readonly IUserService _userService;

        public UpdateProfilePatientWindow(Patient currentPatient, IUserService userService, IAppointmentService appointmentService)
        {
            InitializeComponent();

            _currentPatient = currentPatient ?? throw new ArgumentNullException(nameof(currentPatient));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            LoadPatientData();
        }

        private void LoadPatientData()
        {
            LastNameTextBox.Text = _currentPatient.LastName;
            FirstNameTextBox.Text = _currentPatient.FirstName;
            MiddleNameTextBox.Text = _currentPatient.MiddleName;
        }

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Text = "";

            string lastName = LastNameTextBox.Text.Trim();
            string firstName = FirstNameTextBox.Text.Trim();
            string middleName = MiddleNameTextBox.Text.Trim();

            bool updateSuccess = _userService.UpdateUserProfile(_currentPatient.Id, firstName, lastName, middleName);

            if (updateSuccess)
            {
                MessageBox.Show("Профіль пацієнта успішно оновлено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                _currentPatient.LastName = lastName;
                _currentPatient.FirstName = firstName;
                _currentPatient.MiddleName = middleName;

                this.Close();

            }
            else
            {
                ErrorTextBlock.Text = "Помилка при оновленні профілю. Перевірте дані та спробуйте ще раз.";
            }
        }
    }
}
