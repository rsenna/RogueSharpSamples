using System;
using GoRogue;
using GoRogueSample3.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using SadConsole.Controls;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GoRogueSample3.UI
{
    // Creates/Holds/Destroys all consoles used in the game
    // and makes consoles easily addressable from a central place.
    public class UIManager : ContainerConsole
    {
        public UIManager()
        {
            // must be set to true
            // or will not call each child's Draw method
            IsVisible = true;
            IsFocused = true;

            // The UIManager becomes the only
            // screen that SadConsole processes
            Parent = Global.CurrentScreen;
        }

        public ScrollingConsole MapConsole { get; set; }
        public Window MapWindow { get; set; }
        public MessageLogWindow MessageLog { get; set; }

        // Creates all child consoles to be managed
        public void CreateConsoles()
        {
            // Temporarily create a console with *no* tile data that will later be replaced with map data
            MapConsole = new ScrollingConsole(Program.GameWidth, Program.GameHeight);
        }

        // centers the viewport camera on an Actor
        public void CenterOnActor(Actor actor)
        {
            MapConsole.CenterViewPortOnPoint(actor.Position);
        }

        public override void Update(TimeSpan timeElapsed)
        {
            CheckKeyboard();
            base.Update(timeElapsed);
        }

        // Scans the SadConsole's Global KeyboardState and triggers behaviour
        // based on the button pressed.
        private void CheckKeyboard()
        {
            // As an example, we'll use the F5 key to make the game full screen
            if (Global.KeyboardState.IsKeyReleased(Keys.F5))
            {
                Settings.ToggleFullScreen();
            }

            // Keyboard movement for Player character: Up arrow
            // Decrement player's Y coordinate by 1
            if (Global.KeyboardState.IsKeyPressed(Keys.Up))
            {
                Program.CommandManager.MoveActorBy(Program.World.Player, new Point(0, -1));
                CenterOnActor(Program.World.Player);
            }

            // Keyboard movement for Player character: Down arrow
            // Increment player's Y coordinate by 1
            if (Global.KeyboardState.IsKeyPressed(Keys.Down))
            {
                Program.CommandManager.MoveActorBy(Program.World.Player, new Point(0, 1));
                CenterOnActor(Program.World.Player);
            }

            // Keyboard movement for Player character: Left arrow
            // Decrement player's X coordinate by 1
            if (Global.KeyboardState.IsKeyPressed(Keys.Left))
            {
                Program.CommandManager.MoveActorBy(Program.World.Player, new Point(-1, 0));
                CenterOnActor(Program.World.Player);
            }

            // Keyboard movement for Player character: Right arrow
            // Increment player's X coordinate by 1
            if (Global.KeyboardState.IsKeyPressed(Keys.Right))
            {
                Program.CommandManager.MoveActorBy(Program.World.Player, new Point(1, 0));
                CenterOnActor(Program.World.Player);
            }

            // Undo last command: Z
            if (Global.KeyboardState.IsKeyReleased(Keys.Z))
            {
                Program.CommandManager.UndoMoveActorBy();
                CenterOnActor(Program.World.Player);
            }

            // Repeat last command: X
            if (Global.KeyboardState.IsKeyPressed(Keys.X))
            {
                Program.CommandManager.RedoMoveActorBy();
                CenterOnActor(Program.World.Player);
            }
        }


        // Initializes all windows and consoles
        public void Init()
        {
            CreateConsoles();

            //Message Log initialization
            MessageLog = new MessageLogWindow(Program.GameWidth, Program.GameHeight / 2, "Message Log");
            Children.Add(MessageLog);
            MessageLog.Show();
            MessageLog.Position = new Point(0, Program.GameHeight / 2);

            MessageLog.Add("Testing 123");
            MessageLog.Add("Testing 123");
            MessageLog.Add("Testing 123");
            MessageLog.Add("Testing 123");
            MessageLog.Add("Testing 123");
            MessageLog.Add("Testing 123");
            MessageLog.Add("Testing 12");
            MessageLog.Add("Testing 1");
            MessageLog.Add("Testing");
            MessageLog.Add("Testing 12");
            MessageLog.Add("Testing 1");
            MessageLog.Add("Testing");
            MessageLog.Add("Testing 12");
            MessageLog.Add("Testing 1");
            MessageLog.Add("Testing Last");

            // Load the map into the MapConsole
            LoadMap(Program.World.CurrentMap);

            // Now that the MapConsole is ready, build the Window
            CreateMapWindow(Program.GameWidth / 2, Program.GameHeight / 2, "Game Map");
            UseMouse = true;

            // Start the game with the camera focused on the player
            CenterOnActor(Program.World.Player);
        }

        // Adds the entire list of entities found in the
        // World.CurrentMap's Entities SpatialMap to the
        // MapConsole, so they can be seen onscreen
        private void SyncMapEntities(Map map)
        {
            // remove all Entities from the console first
            MapConsole.Children.Clear();

            // Now pull all of the entities into the MapConsole in bulk
            foreach (var entity in map.Entities.Items)
            {
                MapConsole.Children.Add(entity);
            }

            // Subscribe to the Entities ItemAdded listener, so we can keep our MapConsole entities in sync
            map.Entities.ItemAdded += OnMapEntityAdded;

            // Subscribe to the Entities ItemRemoved listener, so we can keep our MapConsole entities in sync
            map.Entities.ItemRemoved += OnMapEntityRemoved;
        }

        // Remove an Entity from the MapConsole every time the Map's Entity collection changes
        public void OnMapEntityRemoved(object sender, ItemEventArgs<Entity> args)
        {
            MapConsole.Children.Remove(args.Item);
        }

        // Add an Entity to the MapConsole every time the Map's Entity collection changes
        public void OnMapEntityAdded(object sender, ItemEventArgs<Entity> args)
        {
            MapConsole.Children.Add(args.Item);
        }

        // Loads a Map into the MapConsole
        private void LoadMap(Map map)
        {
            // First load the map's tiles into the console
            MapConsole = new ScrollingConsole(Program.World.CurrentMap.Width, Program.World.CurrentMap.Height, Global.FontDefault, new Rectangle(0, 0, Program.GameWidth, Program.GameHeight), map.Tiles);

            // Now Sync all of the map's entities
            SyncMapEntities(map);
        }

        // Creates a window that encloses a map console
        // of a specified height and width
        // and displays a centered window title
        // make sure it is added as a child of the UIManager
        // so it is updated and drawn
        public void CreateMapWindow(int width, int height, string title)
        {
            MapWindow = new Window(width, height);
            MapWindow.CanDrag = true;

            //make console short enough to show the window title
            //and borders, and position it away from borders
            var mapConsoleWidth = width - 2;
            var mapConsoleHeight = height - 2;

            // Resize the Map Console's ViewPort to fit inside of the window's borders snugly
            MapConsole.ViewPort = new Rectangle(0, 0, mapConsoleWidth, mapConsoleHeight);

            //reposition the MapConsole so it doesnt overlap with the left/top window edges
            MapConsole.Position = new Point(1, 1);

            //close window button
            var closeButton = new Button(3);
            closeButton.Position = new Point(0, 0);
            closeButton.Text = "[X]";

            //Add the close button to the Window's list of UI elements
            MapWindow.Add(closeButton);

            // Centre the title text at the top of the window
            MapWindow.Title = title.Align(HorizontalAlignment.Center, mapConsoleWidth);

            //add the map viewer to the window
            MapWindow.Children.Add(MapConsole);

            // The MapWindow becomes a child console of the UIManager
            Children.Add(MapWindow);

            // Without this, the window will never be visible on screen
            MapWindow.Show();
        }
    }
}
