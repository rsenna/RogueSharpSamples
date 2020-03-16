using Microsoft.Xna.Framework;
using RogueSharp;
using RogueSharpSample2.Interfaces;
using SadConsole;

namespace RogueSharpSample2.Core
{
    public class Gold : ITreasure, Interfaces.IDrawable
    {
        public Gold(int amount)
        {
            Amount = amount;
            Symbol = '$';
            Color = Color.Yellow;
        }

        public int Amount { get; set; }

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

        public bool PickUp(IActor actor)
        {
            actor.Gold += Amount;
            RogueGame.MessageLog.Add($"{actor.Name} picked up {Amount} gold");
            return true;
        }
    }
}
