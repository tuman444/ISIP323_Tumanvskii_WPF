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
    /// Логика взаимодействия для MoviePage.xaml
    /// </summary>
    public partial class MoviePage : Page
    {
        private Movies _currentMovie;
        public MoviePage(Movies selectedMovie)
        {
            InitializeComponent();
            _currentMovie = selectedMovie;
            LoadData();
        }
        private void LoadData()
        {
            TxtTitle.Text = _currentMovie.Title;
            TxtDescription.Text = _currentMovie.Description;
            TxtRating.Text = _currentMovie.Rating.ToString();
            TxtDuration.Text = _currentMovie.Duration.ToString();
            TxtGenres.Text = _currentMovie.GenreString;

            if (!string.IsNullOrEmpty(_currentMovie.ImagePath))
            {
                try
                {
                    ImgMoviePoster.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(_currentMovie.ImagePath, UriKind.RelativeOrAbsolute));
                }
                catch
                {

                }
            }

            var sessions = Core.Context.Sessions.Where(s => s.MovieID == _currentMovie.ID).OrderBy(s => s.DateTime) .ToList();

            GridSessions.ItemsSource = sessions;    
        }

        private void GridSessions_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Для покупки билета необходимо войти в систему!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                NavigationService.Navigate(new LoginPage());
                return;
            }

            var selectedSession = GridSessions.SelectedItem as Sessions;

            if (selectedSession != null)
            {
                NavigationService.Navigate(new Pages.SessionsPage(selectedSession));

                MessageBox.Show($"Выбран сеанс: {selectedSession.DateTime}. Переход к выбору мест...");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
