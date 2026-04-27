using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PR17.Pages.Master
{
    /// <summary>
    /// Логика взаимодействия для AppointmentDetailsPage.xaml
    /// </summary>
    public partial class AppointmentDetailsPage : Page
    {
        private Appointments _currentAppointment;
        public AppointmentDetailsPage(Appointments selectedAppointment)
        {
            InitializeComponent();
            this.DataContext = selectedAppointment;
            if (selectedAppointment == null )
            {
                MessageBox.Show("Данные о записи не найдены");
                return;
            }
            _currentAppointment = selectedAppointment;
            FillDetails();
        }

        private void FillDetails()
        {
            TxtDateTime.Text = _currentAppointment.AppointmentDateTime.ToString("dd MMMM yyyy HH:mm");

            if (_currentAppointment.Users != null)
            {
                TxtClient.Text = _currentAppointment.Users.FullName;
            }
            else
            {
                TxtClient.Text = "ID Клиента: " + _currentAppointment.ClientId;
            }

            if (_currentAppointment.Services != null)
            {
                TxtService.Text = _currentAppointment.Services.Name;
            }
            else
            {
                TxtService.Text = "ID Услуги: " + _currentAppointment.ServiceId;
            }

            TxtStatus.Text = _currentAppointment.Status ?? "Planned";
            TxtComment.Text = _currentAppointment.Comment ?? "Нет комментария";
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack(); 
        }
    }
}
