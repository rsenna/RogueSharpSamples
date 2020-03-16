using GoRogueSample3.Commands;
using GoRogueSample3.UI;
using Microsoft.Xna.Framework;
using Game = SadConsole.Game;

namespace GoRogueSample3
{
    public static class Program
    {
        public const int GameWidth = 80;
        public const int GameHeight = 25;

        // Managers
        // TODO: Should not be public, static coupling
        public static UIManager UIManager { get; private set; }

        // TODO: Should not be public, static coupling
        public static CommandManager CommandManager { get; private set; }

        // TODO: Should not be public, static coupling
        public static World World { get; private set; }

        private static void Main()
        {
            // Setup the engine and create the main window.
            Game.Create("Fonts/IBM.font", GameWidth, GameHeight);

            // Hook the start event so we can add consoles to the system.
            Game.OnInitialize = () =>
            {
                //Instantiate the UIManager
                UIManager = new UIManager();

                //Instantiate a new CommandManager
                CommandManager = new CommandManager();

                // Build the world!
                World = new World();

                // Now let the UIManager create its consoles
                // so they can use the World data
                UIManager.Init();
            };

            // Hook the update event that happens each frame so we can trap keys and respond.
            Game.OnUpdate = time => { };

            // Start the game.
            Game.Instance.Run();

            //
            // Code here will not run until the game window closes.
            //

            Game.Instance.Dispose();
        }
    }
}
