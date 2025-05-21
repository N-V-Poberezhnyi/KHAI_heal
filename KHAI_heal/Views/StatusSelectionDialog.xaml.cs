using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KHAI_heal.Enums;

namespace KHAI_heal.Views
{
    public partial class StatusSelectionDialog : Window
    {
        public AppointmentStatus SelectedStatus { get; private set; }

        public StatusSelectionDialog(AppointmentStatus currentStatus, List<AppointmentStatus> availableStatuses)
        {
            InitializeComponent();

            StatusListBox.ItemsSource = availableStatuses;
            StatusListBox.SelectedItem = currentStatus;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (StatusListBox.SelectedItem is AppointmentStatus selectedStatus)
            {
                SelectedStatus = selectedStatus;
                DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть статус.", "Помилка вибору", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
