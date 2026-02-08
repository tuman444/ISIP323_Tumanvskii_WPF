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
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text) ||
                string.IsNullOrWhiteSpace(TxtLastName.Text) ||
                string.IsNullOrWhiteSpace(TxtLogin.Text) ||
                string.IsNullOrWhiteSpace(PBoxPass.Password))
            {

                MessageBox.Show("Заполните все окна", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PBoxPass.Password != PBoxPassConfirm.Password)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existingUser = Core.Context.Users.FirstOrDefault(u => u.Login == TxtLogin.Text);
            if (existingUser != null)
            {
                MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Users newUser = new Users()
            {
                FirstName = TxtName.Text,
                LastName = TxtLastName.Text,
                Login = TxtLogin.Text,
                Password = PBoxPass.Password
            };

            try
            {
                Core.Context.Users.Add(newUser);
                Core.Context.SaveChanges();

                MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.GoBack();
            }
            catch(System.Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
