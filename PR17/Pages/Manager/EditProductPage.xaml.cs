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
    /// Логика взаимодействия для EditProductPage.xaml
    /// </summary>
    public partial class EditProductPage : Page
    {
        private Products _item = new Products();
        public EditProductPage(Products selected)
        {
            InitializeComponent();            
                
            ComboManuf.ItemsSource = Core.DB.Manufacturers.ToList();
            ComboProductType.ItemsSource = Core.DB.ProductTypes.ToList();   

            if (selected != null)
            {
                _item = selected;
                TxtTitle.Text = "Редактирование";
                TbxName.Text = _item.Name;
                TbxPrice.Text = _item.Price.ToString();
                CbIsFrozen.IsChecked = _item.IsFrozen;
                TbxDiscount.Text = _item.DiscountPercentage.ToString();
            }
            else
            {
                _item = new Products();
                TxtTitle.Text = "Добавление";
                TbxPrice.Text = "0";
                TbxDiscount.Text = "0";
            }
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbxPrice.Text) || string.IsNullOrWhiteSpace(TbxDiscount.Text))
            {
                MessageBox.Show("Заполните цену и скидку (введите 0, если скидки нет)");
                return;
            }

            try
            {
                _item.Name = TbxName.Text;

                // Вместо: _item.Price = decimal.Parse(TbxPrice.Text);
                decimal price;
                if (decimal.TryParse(TbxPrice.Text, out price))
                {
                    _item.Price = price;
                }
                else
                {
                    _item.Price = 0;
                }

                // То же самое для скидки (int)
                int discount;
                if (int.TryParse(TbxDiscount.Text, out discount))
                {
                    _item.DiscountPercentage = discount;
                }
                _item.IsFrozen = CbIsFrozen.IsChecked ?? false;
                _item.Manufacturers = ComboManuf.SelectedItem as Manufacturers;
                _item.ProductTypes = ComboProductType.SelectedItem as ProductTypes;

                if (_item.Id == 0) Core.DB.Products.Add(_item);
                Core.DB.SaveChanges();

                MessageBox.Show("Товар сохранен!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            { 
                var message = ex.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message;
                MessageBox.Show("Ошибка БД: " + message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
