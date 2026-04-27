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
                RefreshData();
            }
        }

        private void RefreshData()
        {
            // Сброс кэша для актуальных данных
            var context = Core.DB;
            DGridProducts.ItemsSource = context.Products.ToList();
            DGridAppointments.ItemsSource = context.Appointments.OrderByDescending(a => a.AppointmentDateTime).ToList();
            DGridOrders.ItemsSource = context.Orders.OrderByDescending(o => o.OrderDate).ToList();
        }

        // --- ТОВАРЫ ---
        private void Button_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new EditProductPage(null));

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Products product)
                NavigationService.Navigate(new EditProductPage(product));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Products product)
            {
                if (MessageBox.Show($"Удалить {product.Name}?", "Вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Core.DB.Products.Remove(product);
                    Core.DB.SaveChanges();
                    RefreshData();
                }
            }
        }

        // --- ЗАПИСИ ---
        private void TbxSearchClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = TbxSearchClient.Text.ToLower();
            DGridAppointments.ItemsSource = Core.DB.Appointments
                .Where(a => a.Users.FullName.ToLower().Contains(search) || a.Users.Phone.Contains(search))
                .ToList();
        }

        private void BtnCancelApp_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Appointments app)
            {
                app.Status = "Отменена";
                Core.DB.SaveChanges();
                RefreshData();
            }
        }

        private void BtnReschedule_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция переноса даты в разработке...");
        }

        // --- ЗАКАЗЫ ---
        private void BtnDeliverOrder_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Orders order)
            {
                order.Status = "Выдан";
                Core.DB.SaveChanges();
                RefreshData();
            }
        }
    }
}
