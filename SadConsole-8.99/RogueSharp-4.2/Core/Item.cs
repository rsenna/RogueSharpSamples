using Microsoft.Xna.Framework;
using RogueSharp;
using RogueSharpSample2.Interfaces;
using SadConsole;

namespace RogueSharpSample2.Core
{
    public class Item : IItem, ITreasure, Interfaces.IDrawable
    {
        public Item()
        {
            Symbol = '!';
            Color = Color.Yellow;
        }

        public Color Color { get; set; }

        public char Symbol { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public void Draw(Console console, IMap map)
        {
            if (!map.IsExplored(X, Y))
            {
                return;
            }

            if (map.IsInFov(X, Y))
            {
                console.Print(X, Y, Symbol.ToString(), Color, Colors.FloorBackgroundFov);
            }
            else
            {
                console.Print(X, Y, Symbol.ToString(), Color.Multiply(Color.Gray, 0.5f), Colors.FloorBackground);
            }
        }

        public string Name { get; protected set; }
        public int RemainingUses { get; protected set; }

        public bool Use()
        {
            return UseItem();
        }

        public bool PickUp(IActor actor)
        {
            var player = actor as Player;

            if (player != null)
            {
                if (player.AddItem(this))
                {
                    RogueGame.MessageLog.Add($"{actor.Name} picked up {Name}");
                    return true;
                }
            }

            return false;
        }

        protected virtual bool UseItem()
        {
            return false;
        }
    }
}
