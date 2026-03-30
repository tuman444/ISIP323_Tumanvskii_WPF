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

            if (!isGameStarted)
            {
                AddImageToScene("dungeon.jpg");
                return;
            }

            if (game.CurrentItemInChest != null)
            {
                AddImageToScene("chest.jpg");
            }
            else if (game.CurrentEnemies.Count > 0)
            {
                foreach (var enemy in game.CurrentEnemies)
                {
                    string imageName = GetEnemyImageName(enemy);
                    AddImageToScene(imageName, enemy);
                }
            }
            else
            {
                AddImageToScene("dungeon.jpg");
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

        private void AddImageToScene(string imageName, Enemy enemy = null)
        {
            try
            {
                Uri imageUri = new Uri($"pack://application:,,,/Images/{imageName}", UriKind.Absolute);

                // Общий контейнер для одного юнита
                StackPanel enemyUnit = new StackPanel
                {
                    Margin = new Thickness(10, 5, 10, 5),
                    Width = 200,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Image img = new Image
                {
                    Source = new BitmapImage(imageUri),
                    Height = 300, 
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(0, 0, 0, 8) 
                };
                enemyUnit.Children.Add(img);

                if (enemy != null)
                {
                    // Контейнер для HP 
                    Border hpContainer = new Border
                    {
                        Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(230, 230, 230)),
                        CornerRadius = new CornerRadius(3),
                        Height = 16,
                        Width = 120,
                        BorderBrush = System.Windows.Media.Brushes.Gray,
                        BorderThickness = new Thickness(1)
                    };

                    // Сетка для наложения текста поверх полоски HP
                    Grid hpGrid = new Grid();

                    // Сама полоска HP 
                    ProgressBar pbHP = new ProgressBar
                    {
                        Minimum = 0,
                        Maximum = enemy.MaxHP,
                        Value = enemy.HP,
                        Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(211, 47, 47)), // Красный
                        Background = System.Windows.Media.Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    };
                    hpGrid.Children.Add(pbHP);

                    // Текст с цифрами поверх полоски
                    TextBlock txtHP = new TextBlock
                    {
                        Text = $"{enemy.HP}/{enemy.MaxHP}",
                        FontSize = 10,
                        FontWeight = FontWeights.Bold,
                        Foreground = System.Windows.Media.Brushes.Black,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    hpGrid.Children.Add(txtHP);

                    hpContainer.Child = hpGrid;
                    enemyUnit.Children.Add(hpContainer);
                }

                // Добавляем готовую карточку на главную панель
                SceneImagesPanel.Children.Add(enemyUnit);
            }
            catch (Exception ex)
            {
                AddToLog($"Ошибка отрисовки {imageName}");
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
            UpdateSceneImage();
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
