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

namespace PR17.Pages.Manager
{
    /// <summary>
    /// Логика взаимодействия для ReschedulePage.xaml
    /// </summary>
    public partial class ReschedulePage : Page
    {
        private Appointments _currentAppointment;
        public ReschedulePage(Appointments appointment)
        {
            InitializeComponent(); // Это должно идти ПЕРВЫМ

            _currentAppointment = appointment;

            // Безопасная инициализация списков
            try
            {
                // Загружаем услуги
                var services = Core.DB.Services.ToList();
                if (CmbService != null)
                    CmbService.ItemsSource = services;

                // Загружаем мастеров (роли 2 и 3)
                var masters = Core.DB.Users.Where(u => u.RoleId == 2 || u.RoleId == 3).ToList();
                if (CmbMaster != null)
                    CmbMaster.ItemsSource = masters;

                // Устанавливаем текущие значения
                TblClientName.Text = appointment.Users.FullName;
                CmbService.SelectedValue = appointment.ServiceId;
                CmbMaster.SelectedValue = appointment.MasterId;
                DpNewDate.SelectedDate = appointment.AppointmentDateTime.Date;

                // Установка времени
                string currentTime = appointment.AppointmentDateTime.ToString("HH:mm");
                foreach (ComboBoxItem item in CmbTime.Items)
                {
                    if (item.Content.ToString() == currentTime)
                    {
                        CmbTime.SelectedItem = item;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }
       

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (DpNewDate.SelectedDate == null || CmbTime.SelectedItem == null ||
                CmbService.SelectedItem == null || CmbMaster.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля!");
                return;
            }

            try
            {
                // Обновляем услугу и мастера
                _currentAppointment.ServiceId = (int)CmbService.SelectedValue;
                _currentAppointment.MasterId = (int)CmbMaster.SelectedValue;

                // Парсим и обновляем дату/время
                string timeStr = (CmbTime.SelectedItem as ComboBoxItem).Content.ToString();
                TimeSpan time = TimeSpan.Parse(timeStr);
                _currentAppointment.AppointmentDateTime = DpNewDate.SelectedDate.Value.Add(time);

                _currentAppointment.Status = "Редактирована";

                Core.DB.SaveChanges();
                MessageBox.Show("Запись успешно обновлена!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();
    }
}
