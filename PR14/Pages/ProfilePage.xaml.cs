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
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
        }
        
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(Core.CurrentUser == null)
            {
                MessageBox.Show("Ошибка авторизации\t Войдите заново", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService.Navigate(new Pages.LoginPage());
                return;
            }
            TxtName.Text = $"{Core.CurrentUser.FirstName} {Core.CurrentUser.LastName}";
            TxtLogin.Text = Core.CurrentUser.Login;
            TxtPassword.Text = Core.CurrentUser.Password;

            try
            {
                int currentUseriD = Core.CurrentUser.ID;
                var nyTickets = Core.Context.Tickets.Where(t => t.UserID == currentUseriD).OrderByDescending(t => t.Sessions.DateTime).ToList();

                if (nyTickets.Count > 0)
                {
                    LViewTickets.ItemsSource = nyTickets;
                    TxtNoTickets.Visibility = Visibility.Collapsed;
                }
                else
                {
                    LViewTickets.ItemsSource = null;
                    TxtNoTickets.Visibility = Visibility.Visible;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка {ex.Message}");
            }
            
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            Core.CurrentUser = null;
            NavigationService.Navigate(new Pages.LoginPage());
        }

        private void BtnMain_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.MainPage());
        }
    }
}
