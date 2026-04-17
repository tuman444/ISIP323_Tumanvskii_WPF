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

namespace PR17.Pages.Shop
{
    /// <summary>
    /// Логика взаимодействия для CartPage.xaml
    /// </summary>
    public partial class CartPage : Page
    {
        public CartPage()
        {
            InitializeComponent();
            RefreshCart();
        }
        private void RefreshCart()
        {
            LBoxCart.ItemsSource = null;
            LBoxCart.ItemsSource = Core.SelectedProducts;

            // Расчеты
            decimal total = Core.SelectedProducts.Sum(p => p.Price);
            // Считаем скидку на основе DiscountPercentage из твоей БД
            decimal discount = Core.SelectedProducts.Sum(p => p.Price * (p.DiscountPercentage / 100m));

            TxtTotalCount.Text = $"{Core.SelectedProducts.Count} шт.";
            TxtTotalDiscount.Text = $"- {discount:N0} ₽";
            TxtFinalPrice.Text = $"{total - discount:N0} ₽";
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).Tag as Products;
            Core.SelectedProducts.Remove(product);
            RefreshCart();
        }
        private void BtnOrder_Click(object sender, RoutedEventArgs e)
        {
            if (Core.SelectedProducts.Count == 0) return;

            MessageBox.Show("Заказ успешно сформирован!");
            Core.SelectedProducts.Clear();
            NavigationService.GoBack();
        }
    }
}
