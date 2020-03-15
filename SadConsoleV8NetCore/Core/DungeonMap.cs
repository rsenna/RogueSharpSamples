﻿using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using RogueSharpSamples.SadConsoleV8NetCore.Interfaces;
using SadConsole;

namespace RogueSharpSamples.SadConsoleV8NetCore.Core
{
    public class DungeonMap : Map
    {
        private readonly List<Monster> _monsters;
        private readonly List<TreasurePile> _treasurePiles;

        public List<Door> Doors { get; }
        public List<Rectangle> Rooms { get; }
        public Stairs StairsDown { get; set; }
        public Stairs StairsUp { get; set; }

        public DungeonMap()
        {
            _monsters = new List<Monster>();
            _treasurePiles = new List<TreasurePile>();
            RogueGame.SchedulingSystem.Clear();

            Rooms = new List<Rectangle>();
            Doors = new List<Door>();
        }

        public void AddMonster(Monster monster)
        {
            _monsters.Add(monster);
            SetIsWalkable(monster.X, monster.Y, false);
            RogueGame.SchedulingSystem.Add(monster);
        }

        public void RemoveMonster(Monster monster)
        {
            _monsters.Remove(monster);
            SetIsWalkable(monster.X, monster.Y, true);
            RogueGame.SchedulingSystem.Remove(monster);
        }

        public Monster GetMonsterAt(int x, int y)
        {
            // BUG: This should be single except sometiems monsters occupy the same space.
            return _monsters.FirstOrDefault(m => m.X == x && m.Y == y);
        }

        public IEnumerable<Point> GetMonsterLocations()
        {
            return _monsters.Select(m => new Point
            {
                X = m.X,
                Y = m.Y
            });
        }

        public IEnumerable<Point> GetMonsterLocationsInFieldOfView()
        {
            return _monsters.Where(monster => IsInFov(monster.X, monster.Y))
                .Select(m => new Point {X = m.X, Y = m.Y});
        }

        public void AddTreasure(int x, int y, ITreasure treasure)
        {
            _treasurePiles.Add(new TreasurePile(x, y, treasure));
        }

        public void AddPlayer(Player player)
        {
            RogueGame.Player = player;
            SetIsWalkable(player.X, player.Y, false);
            UpdatePlayerFieldOfView();
            RogueGame.SchedulingSystem.Add(player);
        }

        public void UpdatePlayerFieldOfView()
        {
            var player = RogueGame.Player;
            ComputeFov(player.X, player.Y, player.Awareness, true);
            foreach (var cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }

        public bool SetActorPosition(Actor actor, int x, int y)
        {
            if (GetCell(x, y).IsWalkable)
            {
                PickUpTreasure(actor, x, y);
                SetIsWalkable(actor.X, actor.Y, true);
                actor.X = x;
                actor.Y = y;
                SetIsWalkable(actor.X, actor.Y, false);
                OpenDoor(actor, x, y);
                if (actor is Player)
                {
                    UpdatePlayerFieldOfView();
                }

                return true;
            }

            return false;
        }

        public Door GetDoor(int x, int y)
        {
            return Doors.SingleOrDefault(d => d.X == x && d.Y == y);
        }

        private void OpenDoor(Actor actor, int x, int y)
        {
            var door = GetDoor(x, y);
            if (door != null && !door.IsOpen)
            {
                door.IsOpen = true;
                var cell = GetCell(x, y);
                SetCellProperties(x, y, true, true, cell.IsExplored);

                RogueGame.MessageLog.Add($"{actor.Name} opened a door");
            }
        }

        public void AddGold(int x, int y, int amount)
        {
            if (amount > 0)
            {
                AddTreasure(x, y, new Gold(amount));
            }
        }

        private void PickUpTreasure(Actor actor, int x, int y)
        {
            var treasureAtLocation = _treasurePiles.Where(g => g.X == x && g.Y == y).ToList();
            foreach (var treasurePile in treasureAtLocation)
            {
                if (treasurePile.Treasure.PickUp(actor))
                {
                    _treasurePiles.Remove(treasurePile);
                }
            }
        }

        public bool CanMoveDownToNextLevel()
        {
            var player = RogueGame.Player;

            return StairsDown.X == player.X && StairsDown.Y == player.Y;
        }

        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            var cell = GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }

        public Point GetRandomLocation()
        {
            var roomNumber = RogueGame.Random.Next(0, Rooms.Count - 1);
            var randomRoom = Rooms[roomNumber];

            if (!DoesRoomHaveWalkableSpace(randomRoom))
            {
                GetRandomLocation();
            }

            return GetRandomLocationInRoom(randomRoom);
        }

        public Point GetRandomLocationInRoom(Rectangle room)
        {
            var x = RogueGame.Random.Next(1, room.Width - 2) + room.X;
            var y = RogueGame.Random.Next(1, room.Height - 2) + room.Y;
            if (!IsWalkable(x, y))
            {
                GetRandomLocationInRoom(room);
            }

            return new Point(x, y);
        }

        public bool DoesRoomHaveWalkableSpace(Rectangle room)
        {
            for (var x = 1; x <= room.Width - 2; x++)
            for (var y = 1; y <= room.Height - 2; y++)
            {
                if (IsWalkable(x + room.X, y + room.Y))
                {
                    return true;
                }
            }

            return false;
        }

        public void Draw(Console mapConsole, Console statConsole, Console inventoryConsole)
        {
            //mapConsole.Clear();
            foreach (var cell in GetAllCells())
            {
                SetConsoleSymbolForCell(mapConsole, cell);
            }

            foreach (var door in Doors)
            {
                door.Draw(mapConsole, this);
            }

            StairsUp.Draw(mapConsole, this);
            StairsDown.Draw(mapConsole, this);

            foreach (var treasurePile in _treasurePiles)
            {
                var drawableTreasure = treasurePile.Treasure as IDrawable;
                drawableTreasure?.Draw(mapConsole, this);
            }

            //statConsole.Clear();
            var i = 0;
            foreach (var monster in _monsters)
            {
                monster.Draw(mapConsole, this);
                if (IsInFov(monster.X, monster.Y))
                {
                    monster.DrawStats(statConsole, i);
                    i++;
                }
            }

            var player = RogueGame.Player;

            player.Draw(mapConsole, this);
            player.DrawStats(statConsole);
            player.DrawInventory(inventoryConsole);
        }

        private void SetConsoleSymbolForCell(Console console, ICell cell)
        {
            if (!cell.IsExplored)
            {
                return;
            }

            if (IsInFov(cell.X, cell.Y))
            {
                if (cell.IsWalkable)
                {
                    console.Print(cell.X, cell.Y, ".", Colors.FloorFov, Colors.FloorBackgroundFov);
                }
                else
                {
                    console.Print(cell.X, cell.Y, "#", Colors.WallFov, Colors.WallBackgroundFov);
                }
            }
            else
            {
                if (cell.IsWalkable)
                {
                    console.Print(cell.X, cell.Y, ".", Colors.Floor, Colors.FloorBackground);
                }
                else
                {
                    console.Print(cell.X, cell.Y, "#", Colors.Wall, Colors.WallBackground);
                }
            }
        }
    }
}
