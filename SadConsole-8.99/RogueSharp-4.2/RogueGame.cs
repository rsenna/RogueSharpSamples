using System;
using Microsoft.Xna.Framework;
using RogueSharp.Random;
using RogueSharpSample2.Core;
using RogueSharpSample2.Items;
using RogueSharpSample2.Systems;
using SadConsole;
using Console = SadConsole.Console;
using Game = SadConsole.Game;

namespace RogueSharpSample2
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

        private static Console _mainContainer;
        private static Console _mapConsole;
        private static Console _messageConsole;
        private static Console _statConsole;
        private static Console _inventoryConsole;

        private static int _mapLevel = 1;
        private static bool _renderRequired = true;

        private static InputState _inputState;

        public RogueGame()
            : base("Fonts/Cheepicus12.font", ScreenWidth + 2, ScreenHeight + 2, null)
        {
            var seed = (int) DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);

            MessageLog = new MessageLog();
            MessageLog.Add($"The rogue arrives on level {_mapLevel}");
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
            base.Initialize();

            IsMouseVisible = true;

            RefreshTitle();
            Window.Position = new Point(100, 100);
            Window.AllowUserResizing = false;

            _mainContainer = new ContainerConsole();
            _mapConsole = new Console(MapWidth, MapHeight);
            _messageConsole = new Console(MessageWidth, MessageHeight);
            _statConsole = new Console(StatWidth, StatHeight);
            _inventoryConsole = new Console(InventoryWidth, InventoryHeight);

            _mainContainer.Position = new Point(1, 1);
            _mapConsole.Position = new Point(0, InventoryHeight);
            _messageConsole.Position = new Point(0, ScreenHeight - MessageHeight);
            _statConsole.Position = new Point(MapWidth, 0);
            _inventoryConsole.Position = new Point(0, 0);

            _mainContainer.Children.Add(_mapConsole);
            _mainContainer.Children.Add(_messageConsole);
            _mainContainer.Children.Add(_statConsole);
            _mainContainer.Children.Add(_inventoryConsole);

            _mainContainer.IsVisible = true;
            _mainContainer.IsFocused = true;
            _mainContainer.UseMouse = true;
            _mainContainer.UseKeyboard = true;

            _mainContainer.Parent = Global.CurrentScreen;
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
                        RefreshTitle();
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

        private string RefreshTitle()
            => Window.Title = $"RogueSharp SadConsole Example Game - Level {_mapLevel}";
    }
}
