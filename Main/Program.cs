using System;

namespace RogueSharp.SadConsole.Playground.Main
{
    public static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (var game = new RogueGame())
            {
                game.Run();
            }
        }
    }
}
