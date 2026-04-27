using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PR17.Pages.Manager
{
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
            // Обновляем контекст, чтобы подтянуть изменения из базы
            var db = Core.DB;
            DGridProducts.ItemsSource = db.Products.ToList();
            DGridAppointments.ItemsSource = db.Appointments.ToList();
            DGridOrders.ItemsSource = db.Orders.ToList();
            DGridServiceTypes.ItemsSource = db.ProductTypes.ToList();
        }

        // --- ТОВАРЫ ---
        private void Button_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new EditProductPage(null));

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Products selected)
                NavigationService.Navigate(new EditProductPage(selected));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Products product)
            {
                if (MessageBox.Show($"Удалить {product.Name}?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Core.DB.Products.Remove(product);
                    Core.DB.SaveChanges();
                    RefreshData();
                }
            }
        }

        // --- ЗАПИСИ (Поиск по ФИО/Телефону) ---
        private void TbxSearchClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = TbxSearchClient.Text.ToLower();
            DGridAppointments.ItemsSource = Core.DB.Appointments
                .Where(a => a.Users.FullName.ToLower().Contains(search) || a.Users.Phone.Contains(search))
                .ToList();
        }

        private void BtnReschedule_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Appointments selectedApp)
            {
                // Переходим на страницу переноса, передавая выбранную запись
                NavigationService.Navigate(new ReschedulePage(selectedApp));
            }
        }

        // Кнопка ОТМЕНИТЬ
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Appointments selectedApp)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите отменить запись клиента {selectedApp.Users.FullName}?",
                                             "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        selectedApp.Status = "Отменена";
                        Core.DB.SaveChanges();

                        // Обновляем данные в таблице, чтобы сразу увидеть "Отменена" красным
                        DGridAppointments.ItemsSource = Core.DB.Appointments.ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при отмене: " + ex.Message);
                    }
                }
            }
        }

        // --- ЗАКАЗЫ (Выдача) ---
        private void BtnDeliverOrder_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Orders order)
            {
                order.Status = "Выдан";
                Core.DB.SaveChanges();
                RefreshData();
            }
        }

        // --- ТИПЫ УСЛУГ (Исправленное добавление) ---
        // Вкладка Типы услуг: Добавление нового типа
        private void BtnAddServiceType_Click(object sender, RoutedEventArgs e)
        {
            // Генерируем временное уникальное имя, чтобы не сработал UNIQUE KEY
            string newTypeName = "Новый тип " + DateTime.Now.ToString("HH:mm:ss");

            // Проверяем на всякий случай в БД
            if (Core.DB.ProductTypes.Any(pt => pt.Name == newTypeName)) return;

            var newEntry = new ProductTypes { Name = newTypeName };
            Core.DB.ProductTypes.Add(newEntry);

            try
            {
                Core.DB.SaveChanges();
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        // Сохранение при редактировании ячейки (Inline editing)
        private void DGridServiceTypes_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Даем UI обновить объект перед сохранением в БД
                Dispatcher.BeginInvoke(new Action(() => {
                    try
                    {
                        Core.DB.SaveChanges();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Такое название уже существует!");
                        RefreshData(); // Откатываем визуально
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private void BtnDeleteType_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is ProductTypes type)
            {
                try
                {
                    Core.DB.ProductTypes.Remove(type);
                    Core.DB.SaveChanges();
                    RefreshData();
                }
                catch
                {
                    MessageBox.Show("Нельзя удалить тип, используемый в товарах!");
                }
            }
        }
    }
}