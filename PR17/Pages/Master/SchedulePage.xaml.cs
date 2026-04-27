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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PR17.Pages.Master
{
    /// <summary>
    /// Логика взаимодействия для SchedulePage.xaml
    /// </summary>
    public partial class SchedulePage : Page
    {
        public SchedulePage()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            DGridSchedule.ItemsSource = Core.DB.Appointments.ToList();
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            // Берем данные из строки, по которой кликнули
            var rowData = button.DataContext;

            // Пробуем превратить их в Appointments
            if (rowData is Appointments selectedAppointment)
            {
                // Если получилось — идем на страницу деталей
                NavigationService.Navigate(new AppointmentDetailsPage(selectedAppointment));
            }
            else
            {
                // ЕСЛИ НЕ ПОЛУЧИЛОСЬ, ВЫВОДИМ ИМЯ РЕАЛЬНОГО ТИПА
                string typeName = rowData != null ? rowData.GetType().Name : "ПУСТО (NULL)";
                MessageBox.Show($"Данные не передались!\nМы ждали Appointments, а таблица выдала: {typeName}");
            }
        }

        private void BtnEditServices_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.Master.MasterServicesPage());
        }
    }
}
