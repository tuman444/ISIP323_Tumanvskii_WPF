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
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();

            // Загружаем данные для фильтров из БД
            var types = Core.DB.ProductTypes.ToList();
            types.Insert(0, new ProductTypes { Name = "Все типы" });
            ComboType.ItemsSource = types;
            ComboType.SelectedIndex = 0;

            var manufacturers = Core.DB.Manufacturers.ToList();
            manufacturers.Insert(0, new Manufacturers { Name = "Все производители" });
            ComboManufacturer.ItemsSource = manufacturers;
            ComboManufacturer.SelectedIndex = 0;

            ComboSort.SelectedIndex = 0;
            UpdateProducts();
        }

        public void UpdateProducts()
        {
            var list = Core.DB.Products.ToList();

            // 1. Фильтр по типу
            if (ComboType.SelectedIndex > 0)
            {
                var selectedType = ComboType.SelectedItem as ProductTypes;
                list = list.Where(p => p.ProductTypeId == selectedType.Id).ToList();
            }

            // 2. Фильтр по производителю
            if (ComboManufacturer.SelectedIndex > 0)
            {
                var selectedMan = ComboManufacturer.SelectedItem as Manufacturers;
                list = list.Where(p => p.ManufacturerId == selectedMan.Id).ToList();
            }

            // 3. Поиск по названию
            if (!string.IsNullOrWhiteSpace(TbxSearch.Text))
                list = list.Where(p => p.Name.ToLower().Contains(TbxSearch.Text.ToLower())).ToList();

            // 4. Сортировка
            if (ComboSort.SelectedIndex == 1) list = list.OrderBy(p => p.Price).ToList();
            else if (ComboSort.SelectedIndex == 2) list = list.OrderByDescending(p => p.Price).ToList();

            LBoxProducts.ItemsSource = list;
            TxtCount.Text = $"Найдено товаров: {list.Count}";
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            UpdateProducts();
        }

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is PR17.Products product)
            {
                MessageBox.Show($"Товар '{product.Name}' добавлен в корзину!");
            }
        }

    }
}
