using Microsoft.Xna.Framework;
using SadConsole;

namespace RogueSharpSample2
{
    public static class Program
    {
        public static void Main()
        {
            using (SadConsole.Game.Instance = new RogueGame())
            {
                SadConsole.Game.Instance.Run();
            }
        }

        public static void TestMain()
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
