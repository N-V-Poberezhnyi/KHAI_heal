using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using KHAI_heal.Interfaces;
using KHAI_heal.Models;
using System.Linq;
using System;
using KHAI_heal.Enums;
using KHAI_heal.Views;


namespace KHAI_heal
{
    public partial class HomePage : Window
    {
        private readonly User _currentUser;
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;

        public HomePage(User currentUser, IUserService userService, IAppointmentService appointmentService)
        {
            InitializeComponent();

            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));

            LoadAndDisplayDoctors();
        }

        private void LoadAndDisplayDoctors(string searchQuery = null)
        {
            List<Doctor> doctorsToDisplay;
            doctorsToDisplay = _userService.FindDoctors(searchQuery);
            doctorsListView.ItemsSource = doctorsToDisplay;
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            LoadAndDisplayDoctors(searchTextBox.Text);
        }

        private void MyAccountButton_Click(object sender, RoutedEventArgs e)
        {
            AccountWindow accountWindow = new AccountWindow(_currentUser, _userService, _appointmentService);
            accountWindow.ShowDialog();

            LoadAndDisplayDoctors(searchTextBox.Text);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вихід із системи", "Вихід", MessageBoxButton.OK, MessageBoxImage.Information);

            LoginWindow loginWindow = new LoginWindow(_userService, _appointmentService);
            loginWindow.Show();
            this.Close();
        }

        private void BookAppointment_Click(object sender, RoutedEventArgs e)
        {
            Button bookButton = sender as Button;
            if (bookButton != null)
            {
                Doctor selectedDoctor = bookButton.DataContext as Doctor;
                if (selectedDoctor != null)
                {
                    if (_currentUser.Role == UserRole.Patient && _currentUser is Patient currentPatient)
                    {
                        AppointmentBookingWindow bookingWindow = new AppointmentBookingWindow(selectedDoctor, currentPatient, _appointmentService, _userService);
                        bookingWindow.ShowDialog();
                        LoadAndDisplayDoctors(searchTextBox.Text);
                    }
                    else
                    {
                        MessageBox.Show("Для запису на прийом необхідно увійти як пацієнт.", "Необхідна авторизація", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }
    }
}
