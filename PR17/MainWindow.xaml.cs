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
using PR17.Pages.Admin;
using PR17.Pages.Auth;
using PR17.Pages.Manager;
using PR17.Pages.Master;
using PR17.Pages.Salon;
using PR17.Pages.Shop;

namespace PR17
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new Pages.Salon.StartPage());

            CheckRolePermissions();

        }

        private void Navigating(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string command)
            {
                // Используем явное создание страниц с учетом наших новых папок
                switch (command)
                {
                    case "Start":
                        MainFrame.Navigate(new Pages.Salon.StartPage());
                        break;
                    case "Shop":
                        MainFrame.Navigate(new Pages.Shop.ProductsPage());
                        break;
                    case "Account":
                        // MainFrame.Navigate(new Pages.Auth.AccountPage()); // Раскомментируй, когда создашь страницу
                        break;
                    case "Master":
                        // MainFrame.Navigate(new Pages.Master.MasterPage());
                        break;
                    case "Manager":
                        // MainFrame.Navigate(new Pages.Manager.ManagerPage());
                        break;
                    case "Admin":
                        // MainFrame.Navigate(new Pages.Admin.AdminPage());
                        break;
                }
            }
        }
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        // Управляем видимостью кнопки "Назад" и обновляем заголовок
        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            BtnBack.Visibility = MainFrame.CanGoBack ? Visibility.Visible : Visibility.Hidden;

            if (MainFrame.Content is Page page)
            {
                TxtPageTitle.Text = page.Title;
                CheckRolePermissions();
            }
        }

        // Проверка ролей 
        public void CheckRolePermissions()
        {
            // 1. Сначала всё скрываем
            BtnMyRecords.Visibility = Visibility.Collapsed;
            BtnMasterRecords.Visibility = Visibility.Collapsed;
            BtnManagerPanel.Visibility = Visibility.Collapsed;
            BtnAdminPanel.Visibility = Visibility.Collapsed;

            // 2. Если никто не авторизован
            if (Core.AuthUser == null)
            {
                TxtCurrentUser.Text = "Вы вошли как: Гость";
                BtnLogin.Content = "Войти в аккаунт";
                BtnLogin.Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E67E22")); // Оранжевый
                return;
            }

            // 3. Если авторизован - меняем кнопку на "Выйти"
            TxtCurrentUser.Text = $"Пользователь: {Core.AuthUser.FullName}";
            BtnLogin.Content = "Выйти";
            BtnLogin.Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E74C3C")); // Красный

            // 4. Показываем нужные кнопки в зависимости от роли (названия ролей бери из БД)
            string role = Core.AuthUser.Roles.Name;

            switch (role)
            {
                case "Клиент":
                    BtnMyRecords.Visibility = Visibility.Visible;
                    break;
                case "Мастер":
                    BtnMasterRecords.Visibility = Visibility.Visible;
                    break;
                case "Менеджер":
                    BtnManagerPanel.Visibility = Visibility.Visible;
                    break;
                case "Администратор":
                    BtnAdminPanel.Visibility = Visibility.Visible;
                    break;
            }
        }

        // Обработка кнопки Входа / Выхода
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (Core.AuthUser == null)
            {
                // Если не в сети -> идем на страницу авторизации
                MainFrame.Navigate(new Pages.Auth.LoginPage());
            }
            else
            {
                // Если в сети -> выходим
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Core.AuthUser = null; // Очищаем сессию
                    CheckRolePermissions(); // Обновляем меню

                    // Очищаем историю фрейма, чтобы нельзя было нажать "Назад" в закрытый профиль
                    while (MainFrame.CanGoBack) { MainFrame.RemoveBackEntry(); }

                    // Перекидываем на главную
                    MainFrame.Navigate(new Pages.Salon.StartPage());
                }
            }
        }
    }
}
