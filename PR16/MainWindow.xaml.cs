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
        private bool isGameStarted = false;
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
            isGameStarted = false; 

            TxtLog.Text = "--- Вы стоите перед входом в темное подземелье ---\n";
            TxtEvent.Text = "Нажмите 'Идти дальше', чтобы начать приключение";

            UpdateSceneImage();
            UpdateUI();
        }

        private void AddToLog(string message)
        {
            TxtLog.Text += message + "\n";
            LogScroll.ScrollToEnd();
        }

        private void UpdateSceneImage()
        {
            SceneImagesPanel.Children.Clear();

            // Если игра еще не начата
            if (!isGameStarted)
            {
                AddImageToScene("dungeon.jpg");
                return; 
            }

            // Если игра идет, проверяем сундуки и врагов
            if (game.CurrentItemInChest != null)
            {
                AddImageToScene("chest.jpg");
            }
            else if (game.CurrentEnemies.Count > 0)
            {
                foreach (var enemy in game.CurrentEnemies)
                {
                    AddImageToScene(GetEnemyImageName(enemy));
                }
            }
            else
            {
                // Если зачистили комнату
                AddImageToScene("dungeon.png");
            }
        }

        private string GetEnemyImageName(Enemy enemy)
        {
            if (enemy is VVG) return "vvg.jpg";
            if (enemy is Kovalsky) return "kovalsky.jpg";
            if (enemy is ArchmageCPP) return "archmagecpp.jpg";
            if (enemy is PestovC) return "pestovc.png";

            if (enemy is Goblin) return "goblin.jpg";
            if (enemy is Skeleton) return "skeleton.jpg";
            if (enemy is Mage) return "mage.jpg";

            return "dungeon.jpg"; 
        }

        private void AddImageToScene(string imageName)
        {
            try
            {
                Uri imageUri = new Uri($"pack://application:,,,/Images/{imageName}", UriKind.Absolute);

                // Создаем новый элемент Image программно
                Image img = new Image
                {
                    Source = new BitmapImage(imageUri),
                    Width = 200, 
                    Height = 200,
                    Margin = new Thickness(10, 0, 10, 0), 
                    Stretch = System.Windows.Media.Stretch.Uniform
                };

                SceneImagesPanel.Children.Add(img);
            }
            catch (Exception ex)
            {
                AddToLog($"[Ошибка загрузки: {imageName}]");
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
                var result = MessageBox.Show("Вы погибли! Начать заново?", "Игра окончена", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes) StartNewGame();
                else Close();
            }
        }

        private void BtnNextRoom_Click(object sender, RoutedEventArgs e)
        {
            isGameStarted = true; 

            game.NextRoom();
            UpdateSceneImage();

            if (game.CurrentItemInChest != null)
            {
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
