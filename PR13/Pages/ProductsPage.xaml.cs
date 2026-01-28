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
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
            LViewProducts.ItemsSource = Core.Context.Products.ToList();
        }

        private void AddToCartBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var selectedProduct = btn.DataContext as Products;

            Core.Cart.Add(selectedProduct);

            MessageBox.Show($"Товар {selectedProduct.Title} добавлен в корзину!");
        }

        private void GoToCartBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new CartPage());
        }
    }
}
