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
using System.Xml.Linq;

namespace PR17.Pages.Admin
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                DGridUsers.ItemsSource = Core.DB.Users.ToList();
            }
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new Pages.Admin.EditUserPage(null));
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            if((sender as Button).Tag is Users selectedUser)
            {
                //NavigationService.Navigate(new Pages.Admin.EditUserPage(selectedUser));
            }
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Users selectedUser)
            {
                if(Core.AuthUser !=  null && selectedUser.Id == Core.AuthUser.Id)
                {
                    MessageBox.Show("Вы не можете удалить свой собствкнный аккаунт!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (MessageBox.Show($"Вы точно хотите удалить пользователя {selectedUser.Phone}?",
                                    "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Core.DB.Users.Remove(selectedUser);
                        Core.DB.SaveChanges();
                        MessageBox.Show("Пользователь успешно удален!");

                        DGridUsers.ItemsSource = Core.DB.Users.ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                    }
                }
            }
        }
    }
}
