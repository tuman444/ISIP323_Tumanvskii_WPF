using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PR15.Pages
{
    /// <summary>
    /// Логика взаимодействия для BuilderPage.xaml
    /// </summary>
    public partial class BuilderPage : Page
    {
        private ObservableCollection<basepart> _buildParts = new ObservableCollection<basepart>();

        public BuilderPage()
        {
            InitializeComponent();
            LvCart.ItemsSource = _buildParts;
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                var manufacturers = Core.DB.manufacturer.ToList();

                manufacturers.Insert(0, new manufacturer { id = 0, name = "Все производители" });

                CmbManufacturer.ItemsSource = manufacturers;
                CmbManufacturer.DisplayMemberPath = "name";
                CmbManufacturer.SelectedIndex = 0;

                UpdateData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void UpdateData()
        {
            var currentParts = Core.DB.basepart.ToList();

            if (!string.IsNullOrWhiteSpace(TbxSearch.Text))
            {
                currentParts = currentParts.Where(p => p.name.ToLower().Contains(TbxSearch.Text.ToLower())).ToList();
            }

            if (CmbManufacturer.SelectedItem is manufacturer selectedManufacturer && selectedManufacturer.id != 0)
            {
                currentParts = currentParts.Where(p => p.manufacturerid == selectedManufacturer.id).ToList();
            }

            LvCatalog.ItemsSource = currentParts;
        }
        private void TbxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateData();
        }

        private void CmbManufacturer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn?.DataContext is basepart selectedPart)
            {
                if (selectedPart.cpu != null && _buildParts.Any(p => p.cpu != null))
                {
                    MessageBox.Show("В сборке уже есть процессор! Сначала удалите текущий.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (selectedPart.motherboard != null && _buildParts.Any(p => p.motherboard != null))
                {
                    MessageBox.Show("Материнская плата уже добавлена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (selectedPart.gpu != null && _buildParts.Any(p => p.gpu != null))
                {
                    MessageBox.Show("Видеокарта уже добавлена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (selectedPart.powersupply != null && _buildParts.Any(p => p.powersupply != null))
                {
                    MessageBox.Show("Блок питания уже добавлен!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (selectedPart.processorcooler != null && _buildParts.Any(p => p.processorcooler != null))
                {
                    MessageBox.Show("Куллер процессора уже добавлен!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (selectedPart.ram != null && _buildParts.Any(p => p.ram != null))
                {
                    MessageBox.Show("Оперативная память уже добавлена! (Если нужен другой объем, выберите комплект)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (selectedPart.PC_case != null && _buildParts.Any(p => p.PC_case != null))
                {
                    MessageBox.Show("Корпус уже добавлен", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _buildParts.Add(selectedPart);
                CalculateTotal();
                CheckCompatibility();
            }
        }
        private void BtnRemove_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn?.DataContext is basepart selectedPart)
            {
                _buildParts.Remove(selectedPart);
                CalculateTotal();
            }
        }
        private void CalculateTotal()
        {
            decimal total = _buildParts.Sum(p => p.price);
            TbTotalPrice.Text = $"{total:N0} Р";
        }
        private void CheckCompatibility()
        {
            var cpuPart = _buildParts.FirstOrDefault(p => p.cpu != null)?.cpu;
            var moboPart = _buildParts.FirstOrDefault(p => p.motherboard != null)?.motherboard;
            var gpuPart = _buildParts.FirstOrDefault(p => p.gpu != null)?.gpu;
            var ramPart = _buildParts.FirstOrDefault(p => p.ram != null)?.ram;
            var psuPART = _buildParts.FirstOrDefault(P => P.powersupply != null)?.powersupply;

            if (cpuPart != null && moboPart != null)
            {
                if (cpuPart.socketid != moboPart.socketid)
                {
                    MessageBox.Show("Ошибка: Сокет процессора не совпадает с сокетом материнской платы!",
                        "Ошибка совместимости", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            if (moboPart != null && ramPart != null)
            {
                if (moboPart.memorytypeid != ramPart.memorytypeid)
                {
                    MessageBox.Show("Ошибка: Тип оперативной памяти не поддерживается материнской платой!",
                                    "Ошибка совместимости", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            if (psuPART != null)
            {
                int requiredPower = 0;

                if (gpuPart != null && gpuPart.recommendpower != null)
                {
                    requiredPower = gpuPart.recommendpower.Value;
                }
                else
                {
                    // Если видеокарты нет (или у неё нет рек. мощности), считаем базовое потребление
                    if (cpuPart != null) requiredPower += cpuPart.thermalpower;
                    requiredPower += 100;
                }

                if (requiredPower > psuPART.power)
                {
                    MessageBox.Show($"Внимание: Рекомендуемая мощность БП для вашей сборки от {requiredPower}W, " +
                            $"Выбранный блок питания выдает {psuPART.power}W. Возможны отключения ПК под нагрузкой!",
                            "Неподходящий блок питания", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
        }
        private void BtnSaveAssembly_Click(object sender, RoutedEventArgs e)
        {
            List<string> missingParts = new List<string>();

            if (!_buildParts.Any(p => p.cpu != null)) missingParts.Add("- Процессор");
            if (!_buildParts.Any(p => p.motherboard != null)) missingParts.Add("- Материнская плата");
            if (!_buildParts.Any(p => p.ram != null)) missingParts.Add("- Оперативная память");
            if (!_buildParts.Any(p => p.powersupply != null)) missingParts.Add("- Блок питания");
            if (!_buildParts.Any(p => p.storagedevice != null)) missingParts.Add("- Накопитель (SSD или HDD)");
            if (!_buildParts.Any(p => p.processorcooler != null)) missingParts.Add("- Охлаждение процессора (Кулер)");
            if (!_buildParts.Any(p => p.PC_case != null)) missingParts.Add("- Корпус");
            if (!_buildParts.Any(p => p.gpu != null)) missingParts.Add("- Видеокарта");

            if (missingParts.Count > 0)
            {
                string errorMessge = "Сборка неполная! Для работы ПК не хватает:\n\n" + string.Join("\n", missingParts);
                MessageBox.Show(errorMessge, "Ошибка сборки", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TbAssemblyName.Text) || string.IsNullOrWhiteSpace(TbAuthorName.Text))
            {
                MessageBox.Show("Введите название сборки и имя автора!");
                return;
            }

            if (_buildParts.Count == 0)
            {
                MessageBox.Show("Корзина пуста. Нечего сохранять.");
                return;
            }

            try
            {
                using (var db = new PCdbEntities())
                {
                    var newAssembly = new assembly
                    {
                        name = TbAssemblyName.Text,
                        author = TbAuthorName.Text
                    };

                    db.assembly.Add(newAssembly);
                    db.SaveChanges();

                    foreach (var part in _buildParts)
                    {
                        var link = new partassembly
                        {
                            assemblyid = newAssembly.id,
                            partid = part.id
                        };
                        db.partassembly.Add(link);
                    }

                    db.SaveChanges();

                    MessageBox.Show($"Сборка '{newAssembly.name}' успешно сохранена!",
                                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    _buildParts.Clear();
                    TbAssemblyName.Clear();
                    TbAuthorName.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }
    }
}
