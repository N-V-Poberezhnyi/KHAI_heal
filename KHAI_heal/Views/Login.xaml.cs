using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KHAI_heal.Interfaces;
using KHAI_heal.Models;
using KHAI_heal.Services;

namespace KHAI_heal
{
    public partial class LoginWindow : Window
    {
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;

        public LoginWindow(IUserService userService, IAppointmentService appointmentService)
        {
            InitializeComponent();
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            ErrorTextBlock.Text = "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ErrorTextBlock.Text = "Будь ласка, введіть Email та пароль.";
                return;
            }

            User authenticatedUser = _userService.AuthenticateUser(email, password);

            if (authenticatedUser != null)
            {
                MessageBox.Show($"Вітаємо, {authenticatedUser.LastName} {authenticatedUser.FirstName}!", "Успішний вхід", MessageBoxButton.OK, MessageBoxImage.Information);

                HomePage homePage = new HomePage(authenticatedUser, _userService, _appointmentService);
                homePage.Show();
                this.Close();

            }
            else
            {
                ErrorTextBlock.Text = "Невірний Email або пароль.";
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow(_userService, this);
            registrationWindow.Show();
            this.Hide();
        }
    }
}