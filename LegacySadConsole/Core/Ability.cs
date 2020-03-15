using Microsoft.Xna.Framework;
using RogueSharp;
using RogueSharpSamples.LegacySadConsole.Interfaces;
using SadConsole.Consoles;

namespace RogueSharpSamples.LegacySadConsole.Core
{
    public abstract class Ability : IAbility, ITreasure, Interfaces.IDrawable
    {
        protected Ability()
        {
            Symbol = '*';
            Color = Color.Yellow;
        }

        public string Name { get; protected set; }

        public int TurnsToRefresh { get; protected set; }

        public int TurnsUntilRefreshed { get; protected set; }

        public bool Perform()
        {
            if (TurnsUntilRefreshed > 0)
            {
                return false;
            }

            TurnsUntilRefreshed = TurnsToRefresh;

            return PerformAbility();
        }

        public void Tick()
        {
            if (TurnsUntilRefreshed > 0)
            {
                TurnsUntilRefreshed--;
            }
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
                console.CellData.SetCharacter(X, Y, Symbol, Color, Colors.FloorBackgroundFov);
            }
            else
            {
                console.CellData.SetCharacter(X, Y, Symbol, Color.Multiply(Color.Gray, 0.5f), Colors.FloorBackground);
            }
        }

        public bool PickUp(IActor actor)
        {
            var player = actor as Player;

            if (player == null)
            {
                return false;
            }

            if (!player.AddAbility(this))
            {
                return false;
            }

            RogueGame.MessageLog.Add($"{actor.Name} learned the {Name} ability");
            return true;
        }

        protected virtual bool PerformAbility() => false;
    }
}
