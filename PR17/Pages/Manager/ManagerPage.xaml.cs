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

namespace PR17.Pages.Manager
{
    /// <summary>
    /// Логика взаимодействия для ManagerPage.xaml
    /// </summary>
    public partial class ManagerPage : Page
    {
        public ManagerPage()
        {
            InitializeComponent();
        }
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Обновляем список из БД
                DGridProducts.ItemsSource = Core.DB.Products.ToList();
            }
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу редактирования товара
            //NavigationService.Navigate(new Pages.Manager.EditProductPage(null));
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Products selectedProduct)
            {
                //NavigationService.Navigate(new Pages.Manager.EditProductPage(selectedProduct));
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).Tag as Products;

            if (MessageBox.Show($"Удалить товар {product.Name}?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.DB.Products.Remove(product);
                    Core.DB.SaveChanges();
                    DGridProducts.ItemsSource = Core.DB.Products.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }
    }
}
