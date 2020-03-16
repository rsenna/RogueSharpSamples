using System;
using Microsoft.Xna.Framework;
using RogueSharp.Random;
using RogueSharpSamples.SadConsoleV8NetCore.Core;
using RogueSharpSamples.SadConsoleV8NetCore.Items;
using RogueSharpSamples.SadConsoleV8NetCore.Systems;
using SadConsole;
using Console = SadConsole.Console;
using Game = SadConsole.Game;

namespace RogueSharpSamples.SadConsoleV8NetCore
{
    public class RogueGame : Game
    {
        private const int ScreenWidth = 100;
        private const int ScreenHeight = 70;
        private const int MapWidth = 80;
        private const int MapHeight = 48;
        private const int MessageWidth = 80;
        private const int MessageHeight = 11;
        private const int StatWidth = 20;
        private const int StatHeight = 70;
        private const int InventoryWidth = 80;
        private const int InventoryHeight = 11;

        private static Console _mapConsole;
        private static Console _messageConsole;
        private static Console _statConsole;
        private static Console _inventoryConsole;

        private static int _mapLevel = 1;
        private static bool _renderRequired = true;

        private static InputState _inputState;

        public RogueGame()
            : base(string.Empty, ScreenWidth, ScreenHeight, null)
        {
            const string consoleTitle = "RogueSharp SadConsole Example Game - Level 1";

            var seed = (int) DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);

            MessageLog = new MessageLog();
            MessageLog.Add("The rogue arrives on level 1");
            MessageLog.Add($"Level created with seed '{seed}'");

            Player = new Player();
            SchedulingSystem = new SchedulingSystem();

            var mapGenerator = new MapGenerator(MapWidth, MapHeight, 20, 13, 7, _mapLevel);
            DungeonMap = mapGenerator.CreateMap();

            CommandSystem = new CommandSystem();
            TargetingSystem = new TargetingSystem();

            Player.Item1 = new RevealMapScroll();
            Player.Item2 = new RevealMapScroll();

            _inputState = new InputState();

            Window.Title = consoleTitle;

            Content.RootDirectory = "Content";
        }

        public static Player Player { get; set; }
        public static DungeonMap DungeonMap { get; private set; }
        public static MessageLog MessageLog { get; private set; }
        public static CommandSystem CommandSystem { get; private set; }
        public static SchedulingSystem SchedulingSystem { get; private set; }
        public static TargetingSystem TargetingSystem { get; private set; }
        public static IRandom Random { get; private set; }

        protected override void Initialize()
        {
            IsMouseVisible = true;

            base.Initialize();

            // TODO: Fix font
            // var fontMaster = Global.LoadFont("Fonts/Cheepicus12.font");
            // Global.FontDefault = fontMaster.GetFont(Font.FontSizes.One);

            // TODO: Convert to V8
            // Engine.UseMouse = true;
            // Engine.UseKeyboard = true;

            _mapConsole = new Console(MapWidth, MapHeight);
            _messageConsole = new Console(MessageWidth, MessageHeight);
            _statConsole = new Console(StatWidth, StatHeight);
            _inventoryConsole = new Console(InventoryWidth, InventoryHeight);

            _mapConsole.Position = new Point(0, InventoryHeight);
            _messageConsole.Position = new Point(0, ScreenHeight - MessageHeight);
            _statConsole.Position = new Point(MapWidth, 0);
            _inventoryConsole.Position = new Point(0, 0);

            Global.FocusedConsoles.Push(_messageConsole);
            Global.FocusedConsoles.Push(_statConsole);
            Global.FocusedConsoles.Push(_inventoryConsole);
            Global.FocusedConsoles.Push(_mapConsole);

            // DEBUG:
            var console = _mapConsole;

            console.Fill(new Rectangle(3, 3, 23, 3), Color.Violet, Color.Black, 0, 0);
            console.Print(4, 4, "Hello from SadConsole");

            Global.CurrentScreen = console;
        }

        // TODO: Monsters seem to move slowly, improve
        protected override void Update(GameTime gameTime)
        {
            _renderRequired = true; // Assume render is required by default.
            _inputState.Update(gameTime);

            if (TargetingSystem.IsPlayerTargeting)
            {
                TargetingSystem.HandleInput(_inputState);
            }
            else if (CommandSystem.IsPlayerTurn)
            {
                // TODO: Removed _renderRequired = didPlayerAct optimization, is it needed?
                var didPlayerAct = CommandSystem.HandleInput(_inputState);

                if (didPlayerAct)
                {
                    if (CommandSystem.ExitRequested)
                    {
                        Exit();
                    }
                    else if (CommandSystem.NextLevelRequested)
                    {
                        DungeonMap = new MapGenerator(
                            MapWidth,
                            MapHeight,
                            20,
                            13,
                            7,
                            ++_mapLevel
                        ).CreateMap();

                        MessageLog = new MessageLog();
                        CommandSystem = new CommandSystem();
                        Window.Title = $"RogueSharp SadConsole Example Game - Level {_mapLevel}";
                    }

                    CommandSystem.EndPlayerTurn();
                }
            }
            else
            {
                CommandSystem.ActivateMonsters();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_renderRequired)
            {
                GraphicsDevice.Clear(Color.Black);

                _mapConsole.Clear();
                _messageConsole.Clear();
                _statConsole.Clear();
                _inventoryConsole.Clear();

                MessageLog.Draw(_messageConsole);
                DungeonMap.Draw(_mapConsole, _statConsole, _inventoryConsole);
                MessageLog.Draw(_messageConsole);
                TargetingSystem.Draw(_mapConsole);

                base.Draw(gameTime);
            }
        }
    }
}
