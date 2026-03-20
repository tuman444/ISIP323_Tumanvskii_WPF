using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR16.Models
{
    public class Player
    {
        public int HP { get; private set; }
        public int MaxHP { get; private set; }
        public Weapon CurrentWeapon { get; private set; }
        public Armor CurrentArmor { get; private set; }
        public bool IsFrozen { get; set; }
        public bool IsDefending { get; set; }
        private Random random;

        public Player(int maxHP)
        {
            MaxHP = maxHP;
            HP = maxHP;
            random = new Random();
            CurrentWeapon = new Weapon("Кулаки", 5);
            CurrentArmor = new Armor("Одежда", 2);
        }

        public void Attack(Enemy enemy, Action<string> log)
        {
            if (IsFrozen)
            {
                log("Игрок заморожен и пропускает ход!");
                IsFrozen = false;
                return;
            }

            int damage = CurrentWeapon.Damage;
            enemy.TakeDamage(damage, log);
            log($"Вы атаковали {enemy.Name} и нанесли {damage} урона!");
        }

        public void Defend(Action<string> log)
        {
            if (IsFrozen)
            {
                log("Игрок заморожен и пропускает ход!");
                IsFrozen = false;
                return;
            }
            IsDefending = true;
            log("Вы приготовились к защите!");
        }

        public void TakeDamage(int damage, Action<string> log, bool ignoreArmor = false)
        {
            if (IsDefending)
            {
                if (random.NextDouble() < 0.4)
                {
                    log("Вы увернулись от атаки!");
                    IsDefending = false;
                    return;
                }

                if (!ignoreArmor)
                {
                    double blockPercent = 0.7 + (random.NextDouble() * 0.3);
                    int blockedDamage = (int)(CurrentArmor.Defense * blockPercent);
                    damage = Math.Max(0, damage - blockedDamage);
                    log($"Вы заблокировали {blockedDamage} урона!");
                }
                IsDefending = false;
            }

            HP = Math.Max(0, HP - damage);
            log($"Вы получили {damage} урона. Осталось HP: {HP}");
        }

        public void FullHeal(Action<string> log)
        {
            HP = MaxHP;
            log("Вы выпили зелье и полностью восстановили здоровье!");
        }

        public void EquipWeapon(Weapon weapon, Action<string> log)
        {
            CurrentWeapon = weapon;
            log($"Экипировано оружие: {weapon.GetInfo()}");
        }

        public void EquipArmor(Armor armor, Action<string> log)
        {
            CurrentArmor = armor;
            log($"Экипированы доспехи: {armor.GetInfo()}");
        }

        public bool IsAlive() => HP > 0;
    }
}
