using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR16.Models
{
    public abstract class Item
    {
        public string Name { get; protected set; }
        public Item(string name) => Name = name;

        public abstract void Use(Player player, Action<string> log);
        public virtual string GetInfo() => Name;
    }

    public class Weapon : Item
    {
        public int Damage { get; private set; }
        public Weapon(string name, int damage) : base(name) => Damage = damage;

        public override void Use(Player player, Action<string> log)
        {
            player.EquipWeapon(this, log);
        }
        public override string GetInfo() => $"{Name} (Урон: {Damage})";
    }

    public class Armor : Item
    {
        public int Defense { get; private set; }
        public Armor(string name, int defense) : base(name) => Defense = defense;

        public override void Use(Player player, Action<string> log)
        {
            player.EquipArmor(this, log);
        }
        public override string GetInfo() => $"{Name} (Защита: {Defense})";
    }

    public class Potion : Item
    {
        public Potion(string name) : base(name) { }

        public override void Use(Player player, Action<string> log)
        {
            player.FullHeal(log);
        }

        public override string GetInfo()
        {
            return $"{Name} (Полное восстановление HP)";
        }
    }
}
