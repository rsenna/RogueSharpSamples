﻿using System;
using System.Collections.Generic;
using System.Linq;
using GoRogueSample3.Tiles;
using Microsoft.Xna.Framework;

namespace GoRogueSample3
{
    // based on tunnelling room generation algorithm
    // from RogueSharp tutorial
    // https://roguesharp.wordpress.com/2016/03/26/roguesharp-v3-tutorial-simple-room-generation/
    public class MapGenerator
    {
        private Map _map; // Temporarily store the map currently worked on

        // Create a random number generator
        private readonly Random _random = new Random();

        public Map GenerateMap(int mapWidth, int mapHeight, int maxRooms, int minRoomSize, int maxRoomSize)
        {
            // create an empty map of size (mapWidth x mapHeight)
            _map = new Map(mapWidth, mapHeight);

            // store a list of the rooms created so far
            var rooms = new List<Rectangle>();

            // create up to (maxRooms) rooms on the map
            // and make sure the rooms do not overlap with each other
            for (var i = 0; i < maxRooms; i++)
            {
                // set the room's (width, height) as a random size between (minRoomSize, maxRoomSize)
                var newRoomWidth = _random.Next(minRoomSize, maxRoomSize);
                var newRoomHeight = _random.Next(minRoomSize, maxRoomSize);

                // sets the room's X/Y Position at a random point between the edges of the map
                var newRoomX = _random.Next(0, mapWidth - newRoomWidth - 1);
                var newRoomY = _random.Next(0, mapHeight - newRoomHeight - 1);

                // create a Rectangle representing the room's perimeter
                var newRoom = new Rectangle(newRoomX, newRoomY, newRoomWidth, newRoomHeight);

                // Does the new room intersect with other rooms already generated?
                var newRoomIntersects = rooms.Any(room => newRoom.Intersects(room));

                if (!newRoomIntersects)
                {
                    rooms.Add(newRoom);
                }
            }

            // This is a dungeon, so begin by flooding the map with walls.
            FloodWalls();

            // carve out rooms for every room in the Rooms list
            foreach (var room in rooms)
            {
                CreateRoom(room);
            }

            // carve out tunnels between all rooms
            // based on the Positions of their centers
            for (var r = 1; r < rooms.Count; r++)
            {
                //for all remaining rooms get the center of the room and the previous room
                var previousRoomCenter = rooms[r - 1].Center;
                var currentRoomCenter = rooms[r].Center;

                // give a 50/50 chance of which 'L' shaped connecting hallway to tunnel out
                if (_random.Next(1, 2) == 1)
                {
                    CreateHorizontalTunnel(previousRoomCenter.X, currentRoomCenter.X, previousRoomCenter.Y);
                    CreateVerticalTunnel(previousRoomCenter.Y, currentRoomCenter.Y, currentRoomCenter.X);
                }
                else
                {
                    CreateVerticalTunnel(previousRoomCenter.Y, currentRoomCenter.Y, previousRoomCenter.X);
                    CreateHorizontalTunnel(previousRoomCenter.X, currentRoomCenter.X, currentRoomCenter.Y);
                }
            }

            // Create doors now that the tunnels have been carved out
            foreach (var room in rooms)
            {
                CreateDoor(room);
            }

            // spit out the final map
            return _map;
        }

        // Fills the map with walls
        private void FloodWalls()
        {
            for (var i = 0; i < _map.Tiles.Length; i++)
            {
                _map.Tiles[i] = new TileWall();
            }
        }

        // Builds a room composed of walls and floors using the supplied Rectangle
        // which determines its size and position on the map
        // Walls are placed at the perimeter of the room
        // Floors are placed in the interior area of the room
        private void CreateRoom(Rectangle room)
        {
            // Place floors in interior area
            for (var x = room.Left + 1; x < room.Right - 1; x++)
            {
                for (var y = room.Top + 1; y < room.Bottom - 1; y++)
                {
                    CreateFloor(new Point(x, y));
                }
            }

            // Place walls at perimeter
            var perimeter = GetBorderCellLocations(room);
            foreach (var location in perimeter)
            {
                CreateWall(location);
            }
        }

        // Creates a Floor tile at the specified X/Y location
        private void CreateFloor(Point location)
        {
            _map.Tiles[location.ToIndex(_map.Width)] = new TileFloor();
        }

        // Creates a Wall tile at the specified X/Y location
        private void CreateWall(Point location)
        {
            _map.Tiles[location.ToIndex(_map.Width)] = new TileWall();
        }

        // Returns a list of points expressing the perimeter of a rectangle
        private List<Point> GetBorderCellLocations(Rectangle room)
        {
            //establish room boundaries
            var xMin = room.Left;
            var xMax = room.Right;
            var yMin = room.Top;
            var yMax = room.Bottom;

            // build a list of room border cells using a series of
            // straight lines
            var borderCells = GetTileLocationsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(GetTileLocationsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(GetTileLocationsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(GetTileLocationsAlongLine(xMax, yMin, xMax, yMax));

            return borderCells;
        }

        // carve a tunnel out of the map parallel to the x-axis
        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (var x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                CreateFloor(new Point(x, yPosition));
            }
        }

        // carve a tunnel using the y-axis
        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (var y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                CreateFloor(new Point(xPosition, y));
            }
        }

        //Tries to create a TileDoor object in a specified Rectangle
        //perimeter. Reads through the entire list of tiles comprising
        //the perimeter, and determines if each position is a viable
        //candidate for a door.
        //When it finds a potential position, creates a closed and
        //unlocked door.
        private void CreateDoor(Rectangle room)
        {
            var borderCells = GetBorderCellLocations(room);

            //go through every border cell and look for potential door candidates
            foreach (var location in borderCells)
            {
                var locationIndex = location.ToIndex(_map.Width);
                if (IsPotentialDoor(location))
                {
                    // Create a new door that is closed and unlocked.
                    var newDoor = new TileDoor(false, false);
                    _map.Tiles[locationIndex] = newDoor;
                }
            }
        }

        // Determines if a Point on the map is a good
        // candidate for a door.
        // Returns true if it's a good spot for a door
        // Returns false if there is a Tile that IsBlockingMove=true
        // at that location
        private bool IsPotentialDoor(Point location)
        {
            //if the target location is not walkable
            //then it's a wall and not a good place for a door
            var locationIndex = location.ToIndex(_map.Width);
            if (_map.Tiles[locationIndex] != null && _map.Tiles[locationIndex] is TileWall)
            {
                return false;
            }

            //store references to all neighbouring cells
            var right = new Point(location.X + 1, location.Y);
            var left = new Point(location.X - 1, location.Y);
            var top = new Point(location.X, location.Y - 1);
            var bottom = new Point(location.X, location.Y + 1);

            // check to see if there is a door already in the target
            // location, or above/below/right/left of the target location
            // If it detects a door there, return false.
            if (_map.GetTileAt<TileDoor>(location.X, location.Y) != null ||
                _map.GetTileAt<TileDoor>(right.X, right.Y) != null ||
                _map.GetTileAt<TileDoor>(left.X, left.Y) != null ||
                _map.GetTileAt<TileDoor>(top.X, top.Y) != null ||
                _map.GetTileAt<TileDoor>(bottom.X, bottom.Y) != null
            )
            {
                return false;
            }

            //if all the prior checks are okay, place on left or right side of room
            if (!_map.Tiles[right.ToIndex(_map.Width)].IsBlockingMove && !_map.Tiles[left.ToIndex(_map.Width)].IsBlockingMove && _map.Tiles[top.ToIndex(_map.Width)].IsBlockingMove && _map.Tiles[bottom.ToIndex(_map.Width)].IsBlockingMove)
            {
                return true;
            }

            //if this is a good place for a door at the top or bottom
            if (_map.Tiles[right.ToIndex(_map.Width)].IsBlockingMove && _map.Tiles[left.ToIndex(_map.Width)].IsBlockingMove && !_map.Tiles[top.ToIndex(_map.Width)].IsBlockingMove && !_map.Tiles[bottom.ToIndex(_map.Width)].IsBlockingMove)
            {
                return true;
            }

            return false;
        }

        // returns a collection of Points which represent
        // locations along a line
        public IEnumerable<Point> GetTileLocationsAlongLine(int xOrigin, int yOrigin, int xDestination, int yDestination)
        {
            // prevent line from overflowing
            // boundaries of the map
            xOrigin = ClampX(xOrigin);
            yOrigin = ClampY(yOrigin);
            xDestination = ClampX(xDestination);
            yDestination = ClampY(yDestination);

            var dx = Math.Abs(xDestination - xOrigin);
            var dy = Math.Abs(yDestination - yOrigin);

            var sx = xOrigin < xDestination ? 1 : -1;
            var sy = yOrigin < yDestination ? 1 : -1;
            var err = dx - dy;

            while (true)
            {
                yield return new Point(xOrigin, yOrigin);
                if (xOrigin == xDestination && yOrigin == yDestination)
                {
                    break;
                }

                var e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    xOrigin = xOrigin + sx;
                }

                if (e2 < dx)
                {
                    err = err + dx;
                    yOrigin = yOrigin + sy;
                }
            }
        }

        // sets X coordinate between right and left edges of map
        // to prevent any out-of-bounds errors
        private int ClampX(int x)
        {
            if (x < 0)
            {
                x = 0;
            }
            else if (x > _map.Width - 1)
            {
                x = _map.Width - 1;
            }

            return x;
            // OR using ternary conditional operators: return (x < 0) ? 0 : (x > _map.Width - 1) ? _map.Width - 1 : x;
        }

        // sets Y coordinate between top and bottom edges of map
        // to prevent any out-of-bounds errors
        private int ClampY(int y)
        {
            if (y < 0)
            {
                y = 0;
            }
            else if (y > _map.Height - 1)
            {
                y = _map.Height - 1;
            }

            return y;
            // OR using ternary conditional operators: return (y < 0) ? 0 : (y > _map.Height - 1) ? _map.Height - 1 : y;
        }
    }
}
