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
    /// Логика взаимодействия для OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        public OrderPage()
        {
            InitializeComponent();

            // Ограничение даты: от сегодня до +7 дней
            DpDeliveryDate.DisplayDateStart = DateTime.Today;
            DpDeliveryDate.DisplayDateEnd = DateTime.Today.AddDays(7);
            DpDeliveryDate.SelectedDate = DateTime.Today; // По умолчанию выбран сегодняшний день
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (DpDeliveryDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату доставки!");
                return;
            }

            try
            {
                // 1. Создаем объект заказа на основе вашей таблицы
                Orders newOrder = new Orders
                {
                    ClientId = Core.CurrentUser.Id, // ID текущего клиента
                    OrderDate = DateTime.Now,        // Дата оформления
                    DeliveryDate = DpDeliveryDate.SelectedDate.Value,
                    PaymentMethod = (CmbPayment.SelectedItem as ComboBoxItem).Content.ToString(),
                    Status = "Новый"
                };

                // 2. Добавляем и сохраняем, чтобы получить ID заказа
                Core.DB.Orders.Add(newOrder);
                Core.DB.SaveChanges();

                // 3. Сохраняем товары (связующая таблица)
                // Группируем, чтобы посчитать количество каждого товара
                var groupedProducts = Core.SelectedProducts.GroupBy(p => p.Id);

                foreach (var group in groupedProducts)
                {
                    // Предположим, таблица называется OrderProducts
                    OrderItems op = new OrderItems
                    {
                        OrderId = newOrder.Id,
                        ProductId = group.Key,
                        Quantity = group.Count()
                    };
                    Core.DB.OrderItems.Add(op);
                }

                // Финальное сохранение связей
                Core.DB.SaveChanges();

                MessageBox.Show("Заказ успешно оформлен!");
                Core.SelectedProducts.Clear(); // Очищаем корзину
                NavigationService.GoBack();    // Уходим со страницы оформления
            }
            catch (Exception ex)
            {
                // Вывод ошибки, если что-то пошло не так с БД
                MessageBox.Show("Ошибка при сохранении в БД: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Возврат в корзину без оформления
            NavigationService.GoBack();
        }
    }
}
