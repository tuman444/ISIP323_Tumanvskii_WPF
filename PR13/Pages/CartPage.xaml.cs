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
    /// Логика взаимодействия для CartPage.xaml
    /// </summary>
    public partial class CartPage : Page
    {
        public CartPage()
        {
            InitializeComponent();

            LViewCart.ItemsSource = Core.Cart;
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal sum = Core.Cart.Sum(x => x.Price);
            TotalTxt.Text = $"Итого {sum} рублей";
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void CheckoutBtn_Click(object sender, RoutedEventArgs e)
        {
            if(Core.Cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста");
                return;
            }

            this.NavigationService.Navigate(new OrdersPage());
        }
    }
}
