using Microsoft.Xna.Framework;
using RogueSharp.SadConsole.Playground.Main.Interfaces;
using SadConsole.Consoles;
using IDrawable = RogueSharp.SadConsole.Playground.Main.Interfaces.IDrawable;

namespace RogueSharp.SadConsole.Playground.Main.Core
{
    public class Gold : ITreasure, IDrawable
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
