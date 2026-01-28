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

namespace PR13.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            InitializeComponent();
            decimal sum = Core.Cart.Sum(x => x.Price);
            TotalSumTxt.Text = $"К оплате {sum} руб.";
        }

        private void OrderBtn_Click(object sender, RoutedEventArgs e)
        {
            Orders newOrder = new Orders()
            {
                FullName = TxtName.Text,
                Email = TxtEmail.Text,
                Address = TxtAddress.Text,
                TotalPrice = Core.Cart.Sum(x => x.Price)
            };

            Core.Context.Orders.Add(newOrder);
            Core.Context.SaveChanges();

            foreach (var item in Core.Cart)
            {
                Cart newCartItem = new Cart()
                {
                    ProductId = item.Id,
                    OrderId = newOrder.Id
                };
                Core.Context.Cart.Add(newCartItem);
            }
            Core.Context.SaveChanges();

            MessageBox.Show($"Заказ №{newOrder.Id} успешно оформлен!");

            Core.Cart.Clear();
            this.NavigationService.Navigate(new ProductsPage());
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
