using System;
using Microsoft.Xna.Framework;
using SadConsole.Components;

namespace GoRogueSample3.Entities
{
    //A generic monster capable of
    //combat and interaction
    //yields treasure upon death
    public class Monster : Actor
    {
        private readonly Random rndNum = new Random();

        public Monster(Color foreground, Color background)
            : base(foreground, background, 'M')
        {
            //number of loot to spawn for monster
            var lootNum = rndNum.Next(1, 4);

            for (var i = 0; i < lootNum; i++)
            {
                // monsters are made out of spork, obvs.
                var newLoot = new Item(Color.HotPink, Color.Transparent, "spork", 'L', 2);
                newLoot.Components.Add(new EntityViewSyncComponent());
                Inventory.Add(newLoot);
            }
        }
    }
}
