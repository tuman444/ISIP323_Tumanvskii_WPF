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
    /// Логика взаимодействия для BookingPage.xaml
    /// </summary>
    public partial class BookingPage : Page
    {
        private Sessions _session;
        private List<Seats> _seatsToBuy;
        public BookingPage(Sessions session, List<Seats> seats)
        {
            InitializeComponent();
            _session = session;
            _seatsToBuy = seats;

            LoadData();
        }

        private void LoadData()
        {
            TxtMovieTitle.Text = _session.Movies.Title;
            TxtDateTime.Text = $"{_session.DateTime:f}";
            TxtHall.Text = $"Зал: {_session.Halls.Name}";

            LViewSeats.ItemsSource = _seatsToBuy;

            decimal price = _session.Price ?? 0;
            decimal total = _seatsToBuy.Count * price;
            TxtTotalPrice.Text = $"{total:N0} руб";
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var seat in _seatsToBuy)
                {
                    Tickets newTicket = new Tickets
                    {
                        SessionID = _session.ID,
                        SeatID = seat.ID,
                        UserID = Core.CurrentUser.ID,
                        PurchaseDate = DateTime.Now
                    };
                    Core.Context.Tickets.Add(newTicket);
                }
                Core.Context.SaveChanges();

                MessageBox.Show("Оплата прошла успешно\n Билеты сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.Navigate(new Pages.MainPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
