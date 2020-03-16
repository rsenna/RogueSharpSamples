using RogueSharp;
using SadConsole;

namespace RogueSharpSample2.Core
{
    public class Stairs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsUp { get; set; }

        public void Draw(Console console, IMap map)
        {
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            if (map.IsInFov(X, Y))
            {
                if (IsUp)
                {
                    console.Print(X, Y, "<", Colors.Player);
                }
                else
                {
                    console.Print(X, Y, ">", Colors.Player);
                }
            }
            else
            {
                if (IsUp)
                {
                    console.Print(X, Y, "<", Colors.Floor);
                }
                else
                {
                    console.Print(X, Y, ">", Colors.Floor);
                }
            }
        }
    }
}
