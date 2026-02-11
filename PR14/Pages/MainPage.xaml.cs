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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            CmbSort.SelectedIndex = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateMovies();
            if (Core.CurrentUser != null)
            {
                BtnLogin.Content = "Личный кабинет";
            }
            else
            {
                BtnLogin.Content = "Регистрация/Вход";
            }

        }

        private void UpdateMovies()
        {
            var CurrenMovies = Core.Context.Movies.ToList();

            //Поиск фильма
            if (!string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                CurrenMovies = CurrenMovies.Where(x => x.Title.ToLower().Contains(TxtSearch.Text.ToLower())).ToList();
            }

            if (CmbSort.SelectedIndex == 1)
            {
                CurrenMovies = CurrenMovies.OrderBy(x => x.Title).ToList();
            }
            else if (CmbSort.SelectedIndex == 2)
            {
                CurrenMovies = CurrenMovies.OrderByDescending(x => x.Title).ToList();
            }

            

            LViewMovies.ItemsSource = CurrenMovies;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMovies();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMovies();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser != null)
            {
                NavigationService.Navigate(new Pages.ProfilePage());
            }
            else {
                NavigationService.Navigate(new Pages.LoginPage());
            }
        }

        private void LViewMovies_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedMovie = LViewMovies.SelectedItem as Movies; 
            if (selectedMovie != null)
            {
                NavigationService.Navigate(new Pages.MoviePage(selectedMovie));
            }

        }
    }
}
