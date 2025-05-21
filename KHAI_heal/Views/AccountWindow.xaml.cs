using System.Windows;
using System.Windows.Controls;
using KHAI_heal.Interfaces;
using KHAI_heal.Models;
using KHAI_heal.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media;
using KHAI_heal.Views;


namespace KHAI_heal.Views
{
    public partial class AccountWindow : Window
    {
        private readonly User _currentUser;
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;

        private ObservableCollection<Appointment> _patientAppointments;
        private ObservableCollection<Appointment> _doctorAppointments;

        private TextBlock PublishedStatusTextBlock;
        private Button PublishButton;
        private ListView DoctorScheduleListView;
        private ListView PatientAppointmentsListView;
        private ListView DoctorAppointmentsListView;

        public AccountWindow(User currentUser, IUserService userService, IAppointmentService appointmentService)
        {
            InitializeComponent();

            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));

            _patientAppointments = new ObservableCollection<Appointment>();
            _doctorAppointments = new ObservableCollection<Appointment>();

            DisplayGeneralUserInfo(_currentUser);

            DisplayRoleSpecificInfo(_currentUser);
        }

        private void DisplayGeneralUserInfo(User userToDisplay)
        {
            if (userToDisplay == null) return;
            EmailTextBlock.Text = userToDisplay.Email;
            FullNameTextBlock.Text = $"{userToDisplay.LastName} {userToDisplay.FirstName} {userToDisplay.MiddleName}";
            RoleTextBlock.Text = userToDisplay.Role.ToString();
        }

        private void DisplayRoleSpecificInfo(User userToDisplay)
        {

            if (userToDisplay.Role == UserRole.Patient && userToDisplay is Patient patient)
            {
                DisplayPatientInfo(patient);
            }
            else if (userToDisplay.Role == UserRole.Doctor && userToDisplay is Doctor doctor)
            {
                DisplayDoctorInfo(doctor);
            }
        }

        private void DisplayPatientInfo(Patient patient)
        {
            if (patient == null) return;

            RoleSpecificContentPanel.Children.Clear();

            var appointmentsLabel = new Label { Content = "Мої записи:", FontWeight = FontWeights.Bold, FontSize = 18, Margin = new Thickness(0, 0, 0, 10) };
            RoleSpecificContentPanel.Children.Add(appointmentsLabel);

            PatientAppointmentsListView = new ListView
            {
                ItemsSource = _patientAppointments,
                ItemTemplate = CreateAppointmentDataTemplate()
            };
            RoleSpecificContentPanel.Children.Add(PatientAppointmentsListView);
            LoadPatientAppointments(patient.Id);
        }

        private void DisplayDoctorInfo(Doctor doctor)
        {
            if (doctor == null) return;

            RoleSpecificContentPanel.Children.Clear();

            var doctorPanel = new StackPanel();

            var specializationLabel = new TextBlock { Text = "Спеціалізація:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };
            var specializationValue = new TextBlock { Text = doctor.Specialization.ToString(), Margin = new Thickness(0, 0, 0, 10) };

            var experienceLabel = new TextBlock { Text = "Стаж:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };
            var experienceValue = new TextBlock { Text = $"{doctor.Experience} років", Margin = new Thickness(0, 0, 0, 10) };

            var scheduleLabel = new TextBlock { Text = "Графік прийому:", FontWeight = FontWeights.Bold, FontSize = 18, Margin = new Thickness(0, 0, 0, 10) };
            DoctorScheduleListView = new ListView
            {
                ItemsSource = doctor.Schedule.OrderBy(ts => ts).ToList(),
                DisplayMemberPath = "."
            };

            var doctorAppointmentsLabel = new Label { Content = "Майбутні записи:", FontWeight = FontWeights.Bold, FontSize = 18, Margin = new Thickness(0, 20, 0, 10) };
            DoctorAppointmentsListView = new ListView
            {
                ItemsSource = _doctorAppointments,
                ItemTemplate = CreateAppointmentDataTemplate()
            };

            var publishPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 20, 0, 0) };
            PublishedStatusTextBlock = new TextBlock { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 10, 0) };
            PublishButton = new Button { Content = "Публікувати", Padding = new Thickness(10, 5, 0, 0) };
            PublishButton.Click += PublishButton_Click;

            publishPanel.Children.Add(PublishedStatusTextBlock);
            publishPanel.Children.Add(PublishButton);

            doctorPanel.Children.Add(specializationLabel);
            doctorPanel.Children.Add(specializationValue);

            doctorPanel.Children.Add(experienceLabel);
            doctorPanel.Children.Add(experienceValue);

            doctorPanel.Children.Add(scheduleLabel);
            doctorPanel.Children.Add(DoctorScheduleListView);

            doctorPanel.Children.Add(doctorAppointmentsLabel);
            doctorPanel.Children.Add(DoctorAppointmentsListView);

            doctorPanel.Children.Add(publishPanel);

            RoleSpecificContentPanel.Children.Add(doctorPanel);

            bool isPublished = _userService.GetPublishedStatus(doctor.Id);
            UpdatePublishButtonText(isPublished);

            LoadDoctorAppointments(doctor.Id);
        }

        // Табл. для відображення записів
        private DataTemplate CreateAppointmentDataTemplate()
        {
            var dataTemplate = new DataTemplate();
            dataTemplate.DataType = typeof(Appointment);

            var gridFactory = new FrameworkElementFactory(typeof(Grid));

            var col1 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col1.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Auto)); // Дата/Час

            var col2 = new FrameworkElementFactory(typeof(ColumnDefinition)); // Роздільник
            col2.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Auto));

            var col3 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col3.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star)); // ПІБ

            var col4 = new FrameworkElementFactory(typeof(ColumnDefinition)); // Роздільник
            col4.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Auto));

            var col5 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col5.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Auto)); // Статус

            var col6 = new FrameworkElementFactory(typeof(ColumnDefinition)); // Роздільник
            col6.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Auto));

            var col7 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col7.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Auto)); // Кнопка

            gridFactory.AppendChild(col1);
            gridFactory.AppendChild(col2); // роздільник
            gridFactory.AppendChild(col3);
            gridFactory.AppendChild(col4); // роздільник
            gridFactory.AppendChild(col5);
            gridFactory.AppendChild(col6); // роздільник
            gridFactory.AppendChild(col7); // кнопка зміни статусу

            // TextBlock для дати + часу
            var dateTimeTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            dateTimeTextBlockFactory.SetBinding(TextBlock.TextProperty, new Binding("AppointmentDateTime"));
            dateTimeTextBlockFactory.SetValue(Grid.ColumnProperty, 0);
            dateTimeTextBlockFactory.SetValue(TextBlock.MarginProperty, new Thickness(5, 0, 5, 0));
            dateTimeTextBlockFactory.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

            // Роздільник
            var separator1Factory = new FrameworkElementFactory(typeof(Border));
            separator1Factory.SetValue(Grid.ColumnProperty, 1);
            separator1Factory.SetValue(Border.WidthProperty, 1.0);
            separator1Factory.SetValue(Border.BorderBrushProperty, Brushes.Gray);
            separator1Factory.SetValue(Border.MarginProperty, new Thickness(0, 5, 0, 5));
            separator1Factory.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);


            // TextBlock для ПІБ
            var participantTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            if (_currentUser.Role == UserRole.Patient)
            {
                participantTextBlockFactory.SetBinding(TextBlock.TextProperty, new Binding("DoctorName"));
            }
            else if (_currentUser.Role == UserRole.Doctor)
            {
                participantTextBlockFactory.SetBinding(TextBlock.TextProperty, new Binding("PatientName"));
            }
            participantTextBlockFactory.SetValue(Grid.ColumnProperty, 2);
            participantTextBlockFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            participantTextBlockFactory.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            participantTextBlockFactory.SetValue(TextBlock.MarginProperty, new Thickness(5, 0, 5, 0));

            // Роздільник
            var separator2Factory = new FrameworkElementFactory(typeof(Border));
            separator2Factory.SetValue(Grid.ColumnProperty, 3);
            separator2Factory.SetValue(Border.WidthProperty, 1.0);
            separator2Factory.SetValue(Border.BorderBrushProperty, Brushes.Gray);
            separator2Factory.SetValue(Border.MarginProperty, new Thickness(0, 5, 0, 5));
            separator2Factory.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);

            // TextBlock для статусу
            var statusTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            statusTextBlockFactory.SetBinding(TextBlock.TextProperty, new Binding("Status"));
            statusTextBlockFactory.SetValue(Grid.ColumnProperty, 4);
            statusTextBlockFactory.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            statusTextBlockFactory.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Right);
            statusTextBlockFactory.SetValue(TextBlock.MarginProperty, new Thickness(5, 0, 5, 0));

            // Роздільник
            var separator3Factory = new FrameworkElementFactory(typeof(Border));
            separator3Factory.SetValue(Grid.ColumnProperty, 5);
            separator3Factory.SetValue(Border.WidthProperty, 1.0);
            separator3Factory.SetValue(Border.BorderBrushProperty, Brushes.Gray);
            separator3Factory.SetValue(Border.MarginProperty, new Thickness(0, 5, 0, 5));
            separator3Factory.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);


            // Кнопка для зміни статусу
            var changeStatusButtonFactory = new FrameworkElementFactory(typeof(Button));
            changeStatusButtonFactory.SetValue(Grid.ColumnProperty, 6);
            changeStatusButtonFactory.SetValue(ContentControl.ContentProperty, "Змінити статус");
            changeStatusButtonFactory.SetValue(Button.PaddingProperty, new Thickness(5));
            changeStatusButtonFactory.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            changeStatusButtonFactory.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Right);
            changeStatusButtonFactory.SetValue(Button.MarginProperty, new Thickness(5, 0, 0, 0));
            changeStatusButtonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(AppointmentChangeStatus_Click));

            changeStatusButtonFactory.SetBinding(FrameworkElement.TagProperty, new Binding("Id"));


            gridFactory.AppendChild(dateTimeTextBlockFactory);
            gridFactory.AppendChild(separator1Factory); // роздільник
            gridFactory.AppendChild(participantTextBlockFactory);
            gridFactory.AppendChild(separator2Factory); // роздільник
            gridFactory.AppendChild(statusTextBlockFactory);
            gridFactory.AppendChild(separator3Factory); // роздільник
            gridFactory.AppendChild(changeStatusButtonFactory); // кнопка

            dataTemplate.VisualTree = gridFactory;
            return dataTemplate;
        }

        private void LoadPatientAppointments(int patientId)
        {
            _patientAppointments.Clear();
            List<Appointment> appointments = _appointmentService.GetPatientAppointments(patientId);

            foreach (var appointment in appointments)
            {
                _patientAppointments.Add(appointment);
            }
        }

        private void LoadDoctorAppointments(int doctorId)
        {
            _doctorAppointments.Clear();
            List<Appointment> appointments = _appointmentService.GetDoctorAppointments(doctorId);
            foreach (var appointment in appointments)
            {
                _doctorAppointments.Add(appointment);
            }
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.Role == UserRole.Doctor && _currentUser is Doctor currentDoctor)
            {
                UpdateProfileDoctorWindow updateDoctorWindow = new UpdateProfileDoctorWindow(currentDoctor, _userService, _appointmentService);
                updateDoctorWindow.ShowDialog();

                User updatedUser = _userService.GetUserById(_currentUser.Id);
                if (updatedUser != null)
                {
                    DisplayGeneralUserInfo(updatedUser);

                    if (updatedUser.Role == UserRole.Doctor && updatedUser is Doctor updatedDoctor)
                    {
                        DisplayRoleSpecificInfo(updatedDoctor);
                    }
                    else if (updatedUser.Role == UserRole.Patient && updatedUser is Patient updatedPatient)
                    {
                        DisplayRoleSpecificInfo(updatedPatient);
                    }
                }
            }
            else if (_currentUser.Role == UserRole.Patient && _currentUser is Patient currentPatient)
            {
                UpdateProfilePatientWindow updatePatientWindow = new UpdateProfilePatientWindow(currentPatient, _userService, _appointmentService);

                updatePatientWindow.ShowDialog();

                User updatedUser = _userService.GetUserById(_currentUser.Id);
                if (updatedUser != null && updatedUser is Patient updatedPatient)
                {
                    DisplayGeneralUserInfo(updatedUser);
                    DisplayRoleSpecificInfo(updatedPatient);
                }
            }
            else
            {
                MessageBox.Show("Редагування профілю доступне лише для лікарів та пацієнтів.", "Редагувати профіль", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void PublishButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.Role == UserRole.Doctor && _currentUser is Doctor currentDoctor)
            {
                bool newPublishedStatus = !currentDoctor.IsPublished;

                bool updateSuccess = _userService.UpdateDoctorPublishedStatus(currentDoctor.Id, newPublishedStatus);

                if (updateSuccess)
                {
                    currentDoctor.IsPublished = newPublishedStatus;

                    UpdatePublishButtonText(currentDoctor.IsPublished);

                    MessageBox.Show($"Статус публікації профілю змінено на: {(currentDoctor.IsPublished ? "Опубліковано" : "Не опубліковано")}", "Оновлення статусу", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не вдалося змінити статус публікації. Переконайтесь, що всі необхідні дані профілю заповнені.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdatePublishButtonText(bool isPublished)
        {
            if (PublishedStatusTextBlock != null)
            {
                PublishedStatusTextBlock.Text = isPublished ? "Статус: Опубліковано" : "Статус: Не опубліковано";
            }
            if (PublishButton != null)
            {
                PublishButton.Content = isPublished ? "Зняти з публікації" : "Опублікувати";
            }
        }

        private void AppointmentChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            Button changeStatusButton = sender as Button;
            if (changeStatusButton != null)
            {
                if (changeStatusButton.Tag is int appointmentId)
                {
                    Appointment appointmentToUpdate = null;
                    if (_currentUser.Role == UserRole.Patient)
                    {
                        appointmentToUpdate = _patientAppointments.FirstOrDefault(a => a.Id == appointmentId);
                    }
                    else if (_currentUser.Role == UserRole.Doctor)
                    {
                        appointmentToUpdate = _doctorAppointments.FirstOrDefault(a => a.Id == appointmentId);
                    }


                    if (appointmentToUpdate != null)
                    {
                        if (_currentUser.Role == UserRole.Patient && _currentUser is Patient currentPatient)
                        {
                            if (appointmentToUpdate.Status == AppointmentStatus.Planned)
                            {
                                MessageBoxResult result = MessageBox.Show("Ви впевнені, що хочете скасувати цей запис?", "Підтвердження скасування", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                if (result == MessageBoxResult.Yes)
                                {
                                    bool cancelSuccess = _appointmentService.CancelAppointment(appointmentId, currentPatient.Id);
                                    if (cancelSuccess)
                                    {
                                        MessageBox.Show("Запис успішно скасовано.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                                        LoadPatientAppointments(currentPatient.Id);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Не вдалося скасувати запис.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Ви не можете змінити статус запису зі статусом: {appointmentToUpdate.Status}.", "Дія недоступна", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else if (_currentUser.Role == UserRole.Doctor && _currentUser is Doctor currentDoctor)
                        {
                            List<AppointmentStatus> availableStatuses = new List<AppointmentStatus>();

                            if (appointmentToUpdate.Status == AppointmentStatus.Planned)
                            {
                                availableStatuses.Add(AppointmentStatus.Completed);
                                availableStatuses.Add(AppointmentStatus.Cancelled);
                            }
                            else if (appointmentToUpdate.Status == AppointmentStatus.Completed)
                            {
                                availableStatuses.Add(AppointmentStatus.Cancelled);
                                availableStatuses.Add(AppointmentStatus.Planned);
                            }
                            else if (appointmentToUpdate.Status == AppointmentStatus.Cancelled)
                            {
                                availableStatuses.Add(AppointmentStatus.Planned);
                                availableStatuses.Add(AppointmentStatus.Completed);
                            }

                            if (availableStatuses.Any())
                            {
                                StatusSelectionDialog statusDialog = new StatusSelectionDialog(appointmentToUpdate.Status, availableStatuses);
                                bool? dialogResult = statusDialog.ShowDialog();

                                if (dialogResult == true && statusDialog.SelectedStatus != appointmentToUpdate.Status)
                                {
                                    bool updateSuccess = _appointmentService.UpdateAppointmentStatus(appointmentId, statusDialog.SelectedStatus, currentDoctor.Id);
                                    if (updateSuccess)
                                    {
                                        MessageBox.Show($"Статус запису змінено на: {statusDialog.SelectedStatus}.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                                        LoadDoctorAppointments(currentDoctor.Id);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Не вдалося змінити статус запису.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Ви не можете змінити статус запису зі статусом: {appointmentToUpdate.Status}.", "Дія недоступна", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
            }
        }


    }
}