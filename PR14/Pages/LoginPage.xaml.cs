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

namespace PR14.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLogin.Text) || string.IsNullOrWhiteSpace(PBoxPassword.Password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = Core.Context.Users.FirstOrDefault(u => u.Login == TxtLogin.Text && u.Password == PBoxPassword.Password);

            if(user != null )
            {
                Core.CurrentUser = user;
                MessageBox.Show($"Добро пожаловать {user.FirstName}!" , "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.Navigate(new Pages.MainPage());
            }
            else
            {
                MessageBox.Show("Пользователь не найден, Зарегистрируйтесь", "Ошибка", MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.RegisterPage());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
