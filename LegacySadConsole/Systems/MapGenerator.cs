using System;
using System.Linq;
using RogueSharp;
using RogueSharp.DiceNotation;
using RogueSharpSamples.LegacySadConsole.Core;

namespace RogueSharpSamples.LegacySadConsole.Systems
{
    public class MapGenerator
    {
        private readonly EquipmentGenerator _equipmentGenerator;
        private readonly int _height;
        private readonly int _level;
        private readonly DungeonMap _map;
        private readonly int _maxRooms;
        private readonly int _roomMaxSize;
        private readonly int _roomMinSize;
        private readonly int _width;

        public MapGenerator(int width, int height, int maxRooms, int roomMaxSize, int roomMinSize, int level)
        {
            _width = width;
            _height = height;
            _maxRooms = maxRooms;
            _roomMaxSize = roomMaxSize;
            _roomMinSize = roomMinSize;
            _level = level;
            _map = new DungeonMap();
            _equipmentGenerator = new EquipmentGenerator(level);
        }

        public DungeonMap CreateMap()
        {
            _map.Initialize(_width, _height);

            for (var r = 0; r < _maxRooms; r++)
            {
                var roomWidth = RogueGame.Random.Next(_roomMinSize, _roomMaxSize);
                var roomHeight = RogueGame.Random.Next(_roomMinSize, _roomMaxSize);
                var roomXPosition = RogueGame.Random.Next(0, _width - roomWidth - 1);
                var roomYPosition = RogueGame.Random.Next(0, _height - roomHeight - 1);

                var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);
                var newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));
                if (!newRoomIntersects)
                {
                    _map.Rooms.Add(newRoom);
                }
            }

            foreach (var room in _map.Rooms)
            {
                CreateMap(room);
            }

            for (var r = 0; r < _map.Rooms.Count; r++)
            {
                if (r == 0)
                {
                    continue;
                }

                var previousRoomCenterX = _map.Rooms[r - 1].Center.X;
                var previousRoomCenterY = _map.Rooms[r - 1].Center.Y;
                var currentRoomCenterX = _map.Rooms[r].Center.X;
                var currentRoomCenterY = _map.Rooms[r].Center.Y;

                if (RogueGame.Random.Next(0, 2) == 0)
                {
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, previousRoomCenterY);
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, currentRoomCenterX);
                }
                else
                {
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, previousRoomCenterX);
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, currentRoomCenterY);
                }
            }

            foreach (var room in _map.Rooms)
            {
                CreateDoors(room);
            }

            CreateStairs();

            PlacePlayer();

            PlaceMonsters();

            PlaceEquipment();

            PlaceItems();

            PlaceAbility();

            return _map;
        }

        private void CreateMap(Rectangle room)
        {
            for (var x = room.Left + 1; x < room.Right; x++)
            for (var y = room.Top + 1; y < room.Bottom; y++)
            {
                _map.SetCellProperties(x, y, true, true);
            }
        }

        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (var x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                _map.SetCellProperties(x, yPosition, true, true);
            }
        }

        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (var y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                _map.SetCellProperties(xPosition, y, true, true);
            }
        }

        private void CreateDoors(Rectangle room)
        {
            var xMin = room.Left;
            var xMax = room.Right;
            var yMin = room.Top;
            var yMax = room.Bottom;

            var borderCells = _map.GetCellsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMax, yMin, xMax, yMax));

            foreach (var cell in borderCells)
            {
                if (IsPotentialDoor(cell))
                {
                    _map.SetCellProperties(cell.X, cell.Y, false, true);
                    _map.Doors.Add(new Door
                    {
                        X = cell.X,
                        Y = cell.Y,
                        IsOpen = false
                    });
                }
            }
        }

        private bool IsPotentialDoor(ICell cell)
        {
            if (!cell.IsWalkable)
            {
                return false;
            }

            var right = _map.GetCell(cell.X + 1, cell.Y);
            var left = _map.GetCell(cell.X - 1, cell.Y);
            var top = _map.GetCell(cell.X, cell.Y - 1);
            var bottom = _map.GetCell(cell.X, cell.Y + 1);

            if (_map.GetDoor(cell.X, cell.Y) != null ||
                _map.GetDoor(right.X, right.Y) != null ||
                _map.GetDoor(left.X, left.Y) != null ||
                _map.GetDoor(top.X, top.Y) != null ||
                _map.GetDoor(bottom.X, bottom.Y) != null)
            {
                return false;
            }

            if (right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable)
            {
                return true;
            }

            if (!right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable)
            {
                return true;
            }

            return false;
        }

        private void CreateStairs()
        {
            _map.StairsUp = new Stairs
            {
                X = _map.Rooms.First().Center.X + 1,
                Y = _map.Rooms.First().Center.Y,
                IsUp = true
            };
            _map.StairsDown = new Stairs
            {
                X = _map.Rooms.Last().Center.X,
                Y = _map.Rooms.Last().Center.Y,
                IsUp = false
            };
        }

        private void PlaceMonsters()
        {
            foreach (var room in _map.Rooms)
            {
                if (Dice.Roll("1D10") < 7)
                {
                    var numberOfMonsters = Dice.Roll("1D4");
                    for (var i = 0; i < numberOfMonsters; i++)
                    {
                        if (_map.DoesRoomHaveWalkableSpace(room))
                        {
                            var randomRoomLocation = _map.GetRandomLocationInRoom(room);
                            if (randomRoomLocation != null)
                            {
                                _map.AddMonster(
                                    ActorGenerator.CreateMonster(_level, _map.GetRandomLocationInRoom(room)));
                            }
                        }
                    }
                }
            }
        }

        private void PlaceEquipment()
        {
            foreach (var room in _map.Rooms)
            {
                if (Dice.Roll("1D10") < 3)
                {
                    if (_map.DoesRoomHaveWalkableSpace(room))
                    {
                        var randomRoomLocation = _map.GetRandomLocationInRoom(room);
                        if (randomRoomLocation != null)
                        {
                            Core.Equipment equipment;
                            try
                            {
                                equipment = _equipmentGenerator.CreateEquipment();
                            }
                            catch (InvalidOperationException)
                            {
                                // no more equipment to generate so just quit adding to this level
                                return;
                            }

                            var location = _map.GetRandomLocationInRoom(room);
                            _map.AddTreasure(location.X, location.Y, equipment);
                        }
                    }
                }
            }
        }

        private void PlaceItems()
        {
            foreach (var room in _map.Rooms)
            {
                if (Dice.Roll("1D10") < 3)
                {
                    if (_map.DoesRoomHaveWalkableSpace(room))
                    {
                        var randomRoomLocation = _map.GetRandomLocationInRoom(room);
                        if (randomRoomLocation != null)
                        {
                            var item = ItemGenerator.CreateItem();
                            var location = _map.GetRandomLocationInRoom(room);
                            _map.AddTreasure(location.X, location.Y, item);
                        }
                    }
                }
            }
        }

        private void PlacePlayer()
        {
            var player = ActorGenerator.CreatePlayer();

            player.X = _map.Rooms[0].Center.X;
            player.Y = _map.Rooms[0].Center.Y;

            _map.AddPlayer(player);
        }

        private void PlaceAbility()
        {
            if (_level == 1 || _level % 3 == 0)
            {
                try
                {
                    var ability = AbilityGenerator.CreateAbility();
                    var roomIndex = RogueGame.Random.Next(0, _map.Rooms.Count - 1);
                    var location = _map.GetRandomLocationInRoom(_map.Rooms[roomIndex]);
                    _map.AddTreasure(location.X, location.Y, ability);
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
    }
}
