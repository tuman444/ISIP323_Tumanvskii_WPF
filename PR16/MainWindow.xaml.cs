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
using PR16.Models;
using PR16.Logic;

namespace PR16
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game;
        public MainWindow()
        {
            InitializeComponent();
            StartNewGame();
        }
        private void StartNewGame()
        {
            game = new Game();
            game.OnLogMessage += AddToLog;
            TxtLog.Text = "--- Начало нового приключения ---\n";
            UpdateUI();
        }

        private void AddToLog(string message)
        {
            TxtLog.Text += message + "\n";
            LogScroll.ScrollToEnd();
        }

        private void UpdateSceneImage()
        {
            string imageName = "dungeon.jpg"; // По умолчанию

            if (game.CurrentItemInChest != null)
            {
                imageName = "chest.jpg";
            }
            else if (game.CurrentEnemies.Count > 0)
            {
                // Берем первого врага в списке для отображения
                var enemy = game.CurrentEnemies.First();

                // --- БЛОК ТОЧНОЙ ПРОВЕРКИ БОССОВ (Приоритет) ---
                // Используем старый синтаксис switch/case для C# 7.3

                // Сначала проверяем на боссов, так как они наследуются от обычных врагов
                if (enemy is VVG) imageName = "vvg.jpg";
                else if (enemy is Kovalsky) imageName = "kovalsky.jpg";
                else if (enemy is ArchmageCPP) imageName = "archmagecpp.jpg";
                else if (enemy is PestovC) imageName = "pestovc.png";

                // --- БЛОК ПРОВЕРКИ ОБЫЧНЫХ ВРАГОВ (Если это не босс) ---
                else if (enemy is Goblin) imageName = "goblin.jpg";
                else if (enemy is Skeleton) imageName = "skeleton.jpg";
                else if (enemy is Mage) imageName = "mage.jpg";
            }

            try
            {
                // Пытаемся загрузить изображение
                Uri imageUri = new Uri($"pack://application:,,,/Images/{imageName}", UriKind.Absolute);
                ImgEvent.Source = new BitmapImage(imageUri);
            }
            catch (Exception ex)
            {
                // Если картинка не нашлась, логируем ошибку и ставим заглушку
                AddToLog($"[Ошибка загрузки картинки: {imageName}] - {ex.Message}");
                ImgEvent.Source = null; // Или загрузите дефолтную картинку ошибки
            }
        }

        private void UpdateUI()
        {
            TxtHP.Text = $"HP: {game.Player.HP}/{game.Player.MaxHP}";
            TxtTurn.Text = $"Этаж: {game.TurnCount}";
            TxtWeapon.Text = $"Оружие: {game.Player.CurrentWeapon.GetInfo()}";
            TxtArmor.Text = $"Броня: {game.Player.CurrentArmor.GetInfo()}";

            bool hasEnemies = game.CurrentEnemies.Count > 0;
            bool hasChest = game.CurrentItemInChest != null;

            BtnAttack.Visibility = hasEnemies ? Visibility.Visible : Visibility.Collapsed;
            BtnDefend.Visibility = hasEnemies ? Visibility.Visible : Visibility.Collapsed;
            BtnTake.Visibility = hasChest ? Visibility.Visible : Visibility.Collapsed;
            BtnSkip.Visibility = hasChest ? Visibility.Visible : Visibility.Collapsed;
            BtnNextRoom.Visibility = (!hasEnemies && !hasChest) ? Visibility.Visible : Visibility.Collapsed;

            if (!game.Player.IsAlive())
            {
                // Страница окончания игры по ТЗ
                var result = MessageBox.Show("Вы погибли! Начать заново?", "Игра окончена", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes) StartNewGame();
                else Close();
            }
        }

        private void BtnNextRoom_Click(object sender, RoutedEventArgs e)
        {
            game.NextRoom();

            // Теперь, когда комната сгенерирована, обновляем картинку
            UpdateSceneImage();

            if (game.CurrentItemInChest != null)
            {
                // Логика сравнения характеристик (уже была у нас)
                var item = game.CurrentItemInChest;
                string diff = "";
                if (item is Weapon w) diff = $"\n({w.Damage} атк vs {game.Player.CurrentWeapon.Damage} атк)";
                else if (item is Armor a) diff = $"\n({a.Defense} защ vs {game.Player.CurrentArmor.Defense} защ)";

                TxtEvent.Text = $"В сундуке: {item.Name}{diff}";
            }
            else if (game.CurrentEnemies.Count > 0)
            {
                TxtEvent.Text = $"Враги: {string.Join(", ", game.CurrentEnemies.Select(ev => ev.Name))}";
            }
            else
            {
                TxtEvent.Text = "В комнате пусто и тихо...";
            }
            UpdateUI();
        }

        private void BtnAttack_Click(object sender, RoutedEventArgs e)
        {
            game.PlayerAttack();
            UpdateUI();
        }

        private void BtnDefend_Click(object sender, RoutedEventArgs e)
        {
            game.PlayerDefend();
            UpdateUI();
        }

        private void BtnTake_Click(object sender, RoutedEventArgs e)
        {
            game.TakeItem();
            TxtEvent.Text = "Вы экипировали новый предмет.";
            UpdateUI();
        }

        private void BtnSkip_Click(object sender, RoutedEventArgs e)
        {
            game.LeaveItem();
            TxtEvent.Text = "Вы решили ничего не брать.";
            UpdateUI();
        }
    }
}
