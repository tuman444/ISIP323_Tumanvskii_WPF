using PR16.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR16.Logic
{
    public class Game
    {
        public Player Player { get; private set; }
        public List<Enemy> CurrentEnemies { get; private set; }
        public Item CurrentItemInChest { get; private set; }

        public int TurnCount { get; private set; }
        private Random random;

        public event Action<string> OnLogMessage;

        public Game()
        {
            Player = new Player(100);
            random = new Random();
            TurnCount = 1;
            CurrentEnemies = new List<Enemy>();
        }

        private void Log(string message) => OnLogMessage?.Invoke(message);

        public void NextRoom()
        {
            CurrentEnemies.Clear();
            CurrentItemInChest = null;

            Log($"\n=== Этаж {TurnCount} ===");

            // 1. Проверка на босса (каждые 10 ходов)
            if (TurnCount > 0 && TurnCount % 10 == 0)
            {
                Enemy boss = GenerateRandomBoss();
                CurrentEnemies.Add(boss);
                Log($"!!! ПОЯВИЛСЯ БОСС: {boss.Name} !!!");
            }
            // 2. Шанс 50/50: Сундук или Враги
            else if (random.NextDouble() < 0.5)
            {
                Chest chest = new Chest();
                CurrentItemInChest = chest.Open();
                Log($"*** Вы нашли сундук! Внутри: {CurrentItemInChest.GetInfo()} ***");
            }
            else
            {
                // 3. Генерация группы врагов (от 1 до 3 по ТЗ)
                int enemyCount = random.Next(1, 4);
                for (int i = 0; i < enemyCount; i++)
                {
                    CurrentEnemies.Add(GenerateRandomEnemy());
                }
                Log($"В комнате врагов: {CurrentEnemies.Count}. Приготовьтесь к бою!");
            }
        }

        public void PlayerAttack()
        {
            if (CurrentEnemies.Count > 0)
            {
                // Игрок атакует первого живого врага в списке
                var target = CurrentEnemies.First();
                Player.Attack(target, Log);

                if (!target.IsAlive())
                {
                    Log($"{target.Name} повержен!");
                    CurrentEnemies.Remove(target);
                }

                if (CurrentEnemies.Count == 0)
                {
                    Log("Все враги в комнате зачищены!");
                    TurnCount++;
                }
                else
                {
                    EnemiesTurn();
                }
            }
        }

        public void PlayerDefend()
        {
            if (CurrentEnemies.Count > 0)
            {
                Player.Defend(Log);
                EnemiesTurn();
            }
        }

        private void EnemiesTurn()
        {
            Log($"\n--- Ход врагов ---");
            foreach (var enemy in CurrentEnemies)
            {
                if (enemy.IsAlive())
                {
                    enemy.PerformAttack(Player, Log);
                    enemy.SpecialAbility(Player, Log);
                }
            }
        }

        public void TakeItem()
        {
            if (CurrentItemInChest != null)
            {
                CurrentItemInChest.Use(Player, Log);
                CurrentItemInChest = null;
                TurnCount++;
                Log("Вы идете дальше...");
            }
        }

        // Исправленный метод без использования синтаксиса C# 8.0+
        private Enemy GenerateRandomEnemy()
        {
            int type = random.Next(0, 3);
            switch (type)
            {
                case 0: return new Goblin();
                case 1: return new Skeleton();
                case 2: return new Mage();
                default: return new Goblin();
            }
        }

        private Enemy GenerateRandomBoss()
        {
            int type = random.Next(0, 4);
            switch (type)
            {
                case 0: return new VVG();
                case 1: return new Kovalsky();
                case 2: return new ArchmageCPP();
                case 3: return new PestovC();
                default: return new VVG();
            }
        }
    }
}
