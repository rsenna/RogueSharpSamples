using Microsoft.Xna.Framework;
using SadConsole;

namespace RogueSharpSamples.SadConsoleV8NetCore
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            SadConsole.Game.Create(80, 25);
            SadConsole.Game.OnInitialize = Init;
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        private static void Init()
        {
            var console = new Console(80, 25);

            console.FillWithRandomGarbage();
            console.Fill(new Rectangle(3, 3, 23, 3), Color.Violet, Color.Black, 0, 0);
            console.Print(4, 4, "Hello from SadConsole");

            SadConsole.Global.CurrentScreen = console;
        }
    }
}
