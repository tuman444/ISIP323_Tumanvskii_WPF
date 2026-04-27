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
        public class CartItemModel
        {
            public Products Product { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice => Product.Price * Quantity;
        }
        public CartPage()
        {
            InitializeComponent();
            RefreshCart();
        }
        private void RefreshCart()
        {
            // Группируем одинаковые товары из общего списка
            var groupedCart = Core.SelectedProducts
                .GroupBy(p => p.Id)
                .Select(g => new CartItemModel
                {
                    Product = g.First(),
                    Quantity = g.Count()
                }).ToList();

            LBoxCart.ItemsSource = groupedCart;

            // Расчеты
            decimal total = Core.SelectedProducts.Sum(p => p.Price);
            decimal discount = Core.SelectedProducts.Sum(p => p.Price * (p.DiscountPercentage / 100m));

            TxtTotalCount.Text = $"{Core.SelectedProducts.Count} шт.";
            TxtTotalDiscount.Text = $"- {discount:N0} ₽";
            TxtFinalPrice.Text = $"{total - discount:N0} ₽";
        }

        // Кнопка "+"
        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Products prod)
            {
                Core.SelectedProducts.Add(prod); // Добавляем еще один такой же товар
                RefreshCart();
            }
        }

        // Кнопка "-"
        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Products prod)
            {
                var itemToRemove = Core.SelectedProducts.FirstOrDefault(p => p.Id == prod.Id);
                if (itemToRemove != null)
                {
                    Core.SelectedProducts.Remove(itemToRemove);
                }
                RefreshCart();
            }
        }

        // Полное удаление позиции
        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Products prod)
            {
                // Удаляем все экземпляры этого товара из корзины
                Core.SelectedProducts.RemoveAll(p => p.Id == prod.Id);
                RefreshCart();
            }
        }

        private void BtnOrder_Click(object sender, RoutedEventArgs e)
        {
            if (Core.SelectedProducts.Count == 0) return;

            // Переходим на страницу оформления заказа
            NavigationService.Navigate(new OrderPage());
        }
    }
}
