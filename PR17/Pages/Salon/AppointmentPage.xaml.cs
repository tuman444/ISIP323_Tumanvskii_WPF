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

namespace PR17.Pages.Salon
{
    /// <summary>
    /// Логика взаимодействия для AppointmentPage.xaml
    /// </summary>
    public partial class AppointmentPage : Page
    {
        private Services _selectedService;
        public AppointmentPage(int serviceId)
        {
            InitializeComponent();
            _selectedService = Core.DB.Services.FirstOrDefault(s => s.Id == serviceId);

            if (_selectedService != null )
            {
                TxtServiceName.Text = _selectedService.Name;
                LoadMasters();
            }
        }

        private void LoadMasters()
        {
            // Находим всех пользователей, которые связаны с данной услугой
            // Благодаря EF навигационным свойствам это делается одной строкой
            var masters = _selectedService.Users.ToList();

            if (masters.Count > 0)
            {
                CbMaster.ItemsSource = masters;
                CbMaster.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("На эту услугу пока нет назначенных мастеров.");
                NavigationService.GoBack();
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (DpDate.SelectedDate == null || CbTime.SelectedItem == null ||
                CbPayment.SelectedItem == null || CbMaster.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля!");
                return;
            }

            try
            {
                var selectedMaster = CbMaster.SelectedItem as Users;
                string timeStr = (CbTime.SelectedItem as TextBlock).Text;
                DateTime selectedDateTime = DpDate.SelectedDate.Value.Add(TimeSpan.Parse(timeStr));

                // ПРОВЕРКА: Нет ли уже записи у этого мастера на это время?
                bool isBusy = Core.DB.Appointments.Any(a => a.MasterId == selectedMaster.Id
                                                         && a.AppointmentDateTime == selectedDateTime
                                                         && a.Status != "Cancelled");

                if (isBusy)
                {
                    MessageBox.Show($"Мастер {selectedMaster.FullName} уже занят в это время. Выберите другое.");
                    return;
                }

                // Если свободен — создаем запись
                Appointments newAppointment = new Appointments()
                {
                    ClientId = 6, // Наш тестовый клиент
                    MasterId = selectedMaster.Id,
                    ServiceId = _selectedService.Id,
                    AppointmentDateTime = selectedDateTime,
                    PaymentMethod = (CbPayment.SelectedItem as TextBlock).Text,
                    Comment = TbxComment.Text,
                    Status = "Planned"
                };

                Core.DB.Appointments.Add(newAppointment);
                Core.DB.SaveChanges();

                MessageBox.Show("Запись успешно создана!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
