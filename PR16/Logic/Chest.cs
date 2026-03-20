using PR16.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace PR16.Logic
{
    public class Chest
    {
        private Random random;
        private List<Item> possibleItems;

        public Chest()
        {
            random = new Random();
            possibleItems = new List<Item>
            {
                new Potion("Святая вода"),
                new Potion("Эликсир жизни"),
                new Weapon("Меч", 10),
                new Weapon("Топор", 12),
                new Weapon("Магический посох", 8),
                new Armor("Кожаные доспехи", 5),
                new Armor("Кольчуга", 8),
                new Armor("Латные доспехи", 12)
            };
        }

        public Item Open()
        {
            return possibleItems[random.Next(possibleItems.Count)];
        }
    }
}

