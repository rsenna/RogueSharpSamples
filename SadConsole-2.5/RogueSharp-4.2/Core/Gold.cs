using Microsoft.Xna.Framework;
using RogueSharp;
using RogueSharpSample1.Interfaces;
using SadConsole.Consoles;

namespace RogueSharpSample1.Core
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
                console.CellData.SetCharacter(X, Y, Symbol, Color, Colors.FloorBackgroundFov);
            }
            else
            {
                console.CellData.SetCharacter(X, Y, Symbol, Color.Multiply(Color.Gray, 0.5f), Colors.FloorBackground);
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
