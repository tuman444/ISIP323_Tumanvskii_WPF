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

namespace PR17.Pages.Auth
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            // Это "пинок" главному окну, чтобы оно перерисовало кнопки
            (Application.Current.MainWindow as MainWindow)?.CheckRolePermissions();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string phone = TbxPhone.Text.Trim();
            string pass = PbPassword.Password.Trim();

            if(string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }
            
            var user = Core.DB.Users.FirstOrDefault(u => u.Phone == phone && u.Password == pass);

            if (user != null)
            {
                
                Core.AuthUser = user; // Запоминаем пользователя
                MessageBox.Show($"Добро пожаловать, {user.FullName}!");

                // Навигация в зависимости от роли
                switch (user.Roles.Name)
                {
                    case "Администратор":
                        NavigationService.Navigate(new Admin.AdminPage());
                        break;
                    case "Менеджер":
                        NavigationService.Navigate(new Manager.ManagerPage());
                        break;
                    case "Мастер":
                        NavigationService.Navigate(new Master.SchedulePage());
                        break;
                    case "Клиент":
                        NavigationService.Navigate(new Salon.StartPage());
                        break;
                    default:
                        NavigationService.Navigate(new Salon.StartPage());
                        break;
                }
            }
            else
            {
                MessageBox.Show("Неверный номер телефона или пароль!");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
