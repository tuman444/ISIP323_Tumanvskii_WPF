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
    /// Логика взаимодействия для ProductFullInfoPage.xaml
    /// </summary>
    public partial class ProductFullInfoPage : Page
    {
        private Products _currentProduct;
        public ProductFullInfoPage(Products selectedProduct)
        {
            InitializeComponent();
            if (selectedProduct == null)
            {
                MessageBox.Show("Данные о товаре не найдены");
                if (NavigationService.CanGoBack) NavigationService.GoBack();
                return;
            }

            // Привязываем данные из таблицы Products
            this.DataContext = selectedProduct;
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack(); 
        }
    }
}
