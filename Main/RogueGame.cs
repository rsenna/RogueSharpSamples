using System;
using System.IO;
using Microsoft.Xna.Framework;
using RogueSharp.Random;
using RogueSharp.SadConsole.Playground.Main.Core;
using RogueSharp.SadConsole.Playground.Main.Items;
using RogueSharp.SadConsole.Playground.Main.Systems;
using SadConsole;
using Console = SadConsole.Consoles.Console;

namespace RogueSharp.SadConsole.Playground.Main
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

            var graphics = new GraphicsDeviceManager(this);
            Window.Title = consoleTitle;

            Content.RootDirectory = "Content";
            var sadConsoleComponent = new EngineGameComponent(this, () =>
            {
                using (var stream = File.OpenRead("Fonts/Cheepicus12.font"))
                {
                    Engine.DefaultFont = Serializer.Deserialize<Font>(stream);
                }

                Engine.DefaultFont.ResizeGraphicsDeviceManager(graphics, ScreenWidth, ScreenHeight, 0, 0);
                Engine.UseMouse = true;
                Engine.UseKeyboard = true;

                _mapConsole = new Console(MapWidth, MapHeight);
                _messageConsole = new Console(MessageWidth, MessageHeight);
                _statConsole = new Console(StatWidth, StatHeight);
                _inventoryConsole = new Console(InventoryWidth, InventoryHeight);

                _mapConsole.Position = new Microsoft.Xna.Framework.Point(0, InventoryHeight);
                _messageConsole.Position = new Microsoft.Xna.Framework.Point(0, ScreenHeight - MessageHeight);
                _statConsole.Position = new Microsoft.Xna.Framework.Point(MapWidth, 0);
                _inventoryConsole.Position = new Microsoft.Xna.Framework.Point(0, 0);

                Engine.ConsoleRenderStack.Add(_mapConsole);
                Engine.ConsoleRenderStack.Add(_messageConsole);
                Engine.ConsoleRenderStack.Add(_statConsole);
                Engine.ConsoleRenderStack.Add(_inventoryConsole);

                Engine.ActiveConsole = _mapConsole;
            });

            Components.Add(sadConsoleComponent);
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
        }

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
                var didPlayerAct = _renderRequired = CommandSystem.HandleInput(_inputState);

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

                _mapConsole.CellData.Clear();
                _messageConsole.CellData.Clear();
                _statConsole.CellData.Clear();
                _inventoryConsole.CellData.Clear();

                MessageLog.Draw(_messageConsole);
                DungeonMap.Draw(_mapConsole, _statConsole, _inventoryConsole);
                MessageLog.Draw(_messageConsole);
                TargetingSystem.Draw(_mapConsole);

                base.Draw(gameTime);
            }
        }
    }
}
