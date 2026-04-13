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
    /// Логика взаимодействия для StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();

            LoadFilters();
            Update();
        }
        private void LoadFilters()
        {
            try
            {
                // Загружаем категории (ServiceTypes)
                var types = Core.DB.ServiceTypes.ToList();
                CbType.Items.Clear();
                CbType.Items.Add("Все категории");
                foreach (var t in types) CbType.Items.Add(t.Name);
                CbType.SelectedIndex = 0;

                // Загружаем мастеров (Users с ролью 'Мастер')
                var masters = Core.DB.Users.Where(u => u.Roles.Name == "Мастер").ToList();
                CbMaster.Items.Clear();
                CbMaster.Items.Add("Любой мастер");
                foreach (var m in masters) CbMaster.Items.Add(m.FullName);
                CbMaster.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке фильтров: " + ex.Message);
            }
        }

        private void Update()
        {
            // Работаем с основной таблицей услуг
            var query = Core.DB.Services.AsQueryable();

            // 1. Поиск по названию
            if (!string.IsNullOrWhiteSpace(TbxSearch.Text))
            {
                string search = TbxSearch.Text.ToLower();
                query = query.Where(s => s.Name.ToLower().Contains(search));
            }

            // 2. Фильтр по категории
            if (CbType.SelectedIndex > 0)
            {
                string selectedType = CbType.SelectedItem.ToString();
                query = query.Where(s => s.ServiceTypes.Name == selectedType);
            }

            // 3. Фильтр по мастеру (СВЯЗЬ ЧЕРЕЗ MasterServices)
            if (CbMaster.SelectedIndex > 0)
            {
                string selectedMasterName = CbMaster.SelectedItem.ToString();
                // Находим услуги, которые делает выбранный мастер
                query = query.Where(s => s.Users.Any(u => u.FullName == selectedMasterName));
            }

            LBoxServices.ItemsSource = query.ToList();
        }

        private void FilterChanged(object sender, EventArgs e) => Update();

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                MessageBox.Show($"Услуга ID {id} выбрана. Переходим к записи!");
                NavigationService.Navigate(new Pages.Salon.AppointmentPage(id));
            }
        }
    }
}

