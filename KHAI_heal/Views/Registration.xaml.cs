using System.Windows;
using KHAI_heal.Interfaces;
using KHAI_heal.Models;
using KHAI_heal.Enums;
using System;
using System.Linq;
using System.Collections.Generic;

namespace KHAI_heal
{
    public partial class RegistrationWindow : Window
    {
        private readonly IUserService _userService;
        private readonly Window _loginWindow;

        public RegistrationWindow(IUserService userService, Window loginWindow)
        {
            InitializeComponent();
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _loginWindow = loginWindow ?? throw new ArgumentNullException(nameof(loginWindow));

            RoleComboBox.ItemsSource = Enum.GetValues(typeof(UserRole)).Cast<UserRole>().ToList();
            RoleComboBox.SelectedItem = UserRole.Patient;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string firstName = FirstNameTextBox.Text.Trim();
            string lastName = LastNameTextBox.Text.Trim();
            string middleName = MiddleNameTextBox.Text.Trim();

            UserRole role = UserRole.Patient;
            if (RoleComboBox.SelectedItem is UserRole selectedRole)
            {
                role = selectedRole;
            }
            else
            {
                ErrorTextBlock.Text = "Будь ласка, виберіть роль.";
                return;
            }

            ErrorTextBlock.Text = "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                ErrorTextBlock.Text = "Будь ласка, заповніть всі обов'язкові поля (Email, Пароль, Ім'я, Прізвище).";
                return;
            }
            if (!email.Contains("@") || email.Length < 3 || email.Length > 25)
            {
                ErrorTextBlock.Text = "Будь ласка, введіть коректний Email.";
                return;
            }

            User registeredUser = _userService.RegisterUser(email, password, firstName, lastName, middleName, role);

            if (registeredUser != null)
            {
                MessageBox.Show($"Користувач {registeredUser.Email} успішно зареєстрований!", "Успішна реєстрація", MessageBoxButton.OK, MessageBoxImage.Information);
                _loginWindow.Show();
                this.Close();
            }
            else
            {
                ErrorTextBlock.Text = "Помилка реєстрації. Можливо, Email вже використовується або дані невалідні.";
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            _loginWindow.Show();
            this.Close();
        }
    }
}