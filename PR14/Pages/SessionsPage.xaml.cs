using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PR14.Pages
{
    /// <summary>
    /// Логика взаимодействия для SessionsPage.xaml
    /// </summary>
    public partial class SessionsPage : Page
    {
        private Sessions _currentSession;
        private List<ToggleButton> _selectedButtons = new List<ToggleButton>();
        public SessionsPage(Sessions session)
        {
            InitializeComponent();
            _currentSession = session;
            LoadHall();
        }
        private void LoadHall()
        {
            //Заполняем шапку
            TxtMovieTitle.Text = _currentSession.Movies.Title;
            TxtSessionInfo.Text = $"{_currentSession.DateTime:f} | {_currentSession.Halls.Name}";

            var hallID = _currentSession.HallID;
            var allSeatsInHall = Core.Context.Seats.Where(s => s.HallID == hallID).ToList();

            if (allSeatsInHall.Count == 0)
            {
                MessageBox.Show("В этом зале нет мест", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int maxRow = allSeatsInHall.Max(s => s.RowNumber);
            int maxCol = allSeatsInHall.Max(s => s.SeatNumber);

            UniGridSeats.Rows = maxRow;
            UniGridSeats.Columns = maxCol;

            var soldTickets = Core.Context.Tickets.Where(t => t.SessionID == _currentSession.ID).ToList();

            for (int row = 1; row <= maxRow; row++)
            {
                for (int col = 1; col <= maxCol; col++)
                {
                    var seatInDb = allSeatsInHall.FirstOrDefault(s => s.RowNumber == row && s.SeatNumber == col);

                    if (seatInDb != null)
                    {
                        ToggleButton btn = new ToggleButton();
                        btn.Width = 35;
                        btn.Height = 35;
                        btn.Margin = new Thickness(2);
                        btn.Content = col.ToString();

                        btn.Tag = seatInDb;

                        bool isSold = soldTickets.Any(t => t.SeatID == seatInDb.ID);

                        if (isSold)
                        {
                            btn.IsEnabled = false;
                            btn.Background = Brushes.Gray;
                        }
                        else
                        {
                            btn.Background = Brushes.Violet;
                            btn.Click += BtnSeat_Click;
                        }
                        UniGridSeats.Children.Add(btn);
                    }
                    else
                    {
                        Border spacer = new Border();
                        spacer.Width = 35;
                        spacer.Height = 35;
                        spacer.Margin = new Thickness(2);
                        spacer.Visibility = Visibility.Hidden;
                        UniGridSeats.Children.Add(spacer);
                    }
                }
            }
        }

        private void BtnSeat_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            if(btn.IsChecked == true)
            {
                btn.Background = Brushes.LightCyan;
                _selectedButtons.Add(btn);
            }
            else
            {
                btn.Background= Brushes.Blue;
                _selectedButtons.Remove(btn);
            }
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal price = _currentSession.Price ?? 0; // Цена сеанса
            decimal total = _selectedButtons.Count * price;

            TxtSelectedCount.Text = $"Выбрано: {_selectedButtons.Count}";
            TxtTotalPrice.Text = $"{total:N0} руб.";

        }

        private void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            if(_selectedButtons.Count == 0)
            {
                MessageBox.Show("Выберите места");
                return; 
            }

            if (MessageBox.Show("Оформить покупку?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach(var btn in _selectedButtons)
                    {
                        // Достаем объект Seat из Tag который мы положили туда при создании кнопки
                        var seatData = btn.Tag as Seats;

                        Tickets newTicket = new Tickets
                        {
                            SessionID = _currentSession.ID,
                            SeatID = seatData.ID,
                            UserID = Core.CurrentUser.ID
                        };
                        Core.Context.Tickets.Add(newTicket);
                    }
                    Core.Context.SaveChanges();
                    MessageBox.Show("Билеты куплены");
                    NavigationService.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
