using System;
using GoRogueSample3.Entities;
using GoRogueSample3.Tiles;
using Microsoft.Xna.Framework;
using SadConsole;

namespace GoRogueSample3
{
    // All game state data is stored in World
    // also creates and processes generators
    // for map creation
    public class World
    {
        private TileBase[] _mapTiles;

        // map creation and storage data
        private const int MapHeight = 100;
        private const int MapWidth = 100;
        private const int MaxRooms = 100;
        private const int MaxRoomSize = 15;
        private const int MinRoomSize = 4;

        // random position generator
        private readonly Random _random = new Random();

        // Creates a new game world and stores it in
        // publicly accessible
        public World()
        {
            // Build a map
            CreateMap();

            // create an instance of player
            CreatePlayer();

            // spawn a bunch of monsters
            CreateMonsters();

            // spawn some loot!
            CreateLoot();
        }

        public Map CurrentMap { get; set; }

        // player data
        public Player Player { get; set; }

        // Create a new map using the Map class
        // and a map generator. Uses several
        // parameters to determine geometry
        private void CreateMap()
        {
            _mapTiles = new TileBase[MapWidth * MapHeight];
            CurrentMap = new Map(MapWidth, MapHeight);
            var mapGen = new MapGenerator();
            CurrentMap = mapGen.GenerateMap(MapWidth, MapHeight, MaxRooms, MinRoomSize, MaxRoomSize);
        }

        // Create some random monsters
        // and drop them all over the map in
        // random places.
        private void CreateMonsters()
        {
            // number of monsters to create
            const int numMonsters = 10;

            // Create several monsters with random attack and defense values and
            // pick a random position on the map to place them.
            // check if the placement spot is blocking (e.g. a wall)
            // and if it is, try a new position
            for (var i = 0; i < numMonsters; i++)
            {
                var monsterPosition = 0;
                var newMonster = new Monster(Color.Blue, Color.Transparent);

                while (CurrentMap.Tiles[monsterPosition].IsBlockingMove)
                {
                    // pick a random spot on the map
                    monsterPosition = _random.Next(0, CurrentMap.Width * CurrentMap.Height);
                }

                // plug in some magic numbers for attack and defense values
                newMonster.Defense = _random.Next(0, 10);
                newMonster.DefenseChance = _random.Next(0, 50);
                newMonster.Attack = _random.Next(0, 10);
                newMonster.AttackChance = _random.Next(0, 50);
                newMonster.Name = "a common troll";

                // Set the monster's new position
                // Note: this fancy math will be replaced by a new helper method
                // in the next revision of SadConsole
                newMonster.Position = new Point(monsterPosition % CurrentMap.Width, monsterPosition / CurrentMap.Width);
                CurrentMap.Add(newMonster);
            }
        }

        // Create a player using the Player class
        // and set its starting position
        private void CreatePlayer()
        {
            Player = new Player(Color.Yellow, Color.Transparent);

            // Place the player on the first non-movement-blocking tile on the map
            for (var i = 0; i < CurrentMap.Tiles.Length; i++)
            {
                if (!CurrentMap.Tiles[i].IsBlockingMove)
                {
                    // Set the player's position to the index of the current map position
                    Player.Position = Helpers.GetPointFromIndex(i, CurrentMap.Width);
                    break;
                }
            }

            // add the player to the global EntityManager's collection of Entities
            CurrentMap.Add(Player);
        }

        // Create some sample treasure
        // that can be picked up on the map
        private void CreateLoot()
        {
            // number of treasure drops to create
            const int numLoot = 20;

            // Produce lot up to a max of numLoot
            for (var i = 0; i < numLoot; i++)
            {
                // Create an Item with some standard attributes
                var lootPosition = 0;
                var newLoot = new Item(Color.Green, Color.Transparent, "fancy shirt", 'L', 2);

                // Try placing the Item at lootPosition; if this fails, try random positions on the map's tile array
                while (CurrentMap.Tiles[lootPosition].IsBlockingMove)
                {
                    // pick a random spot on the map
                    lootPosition = _random.Next(0, CurrentMap.Width * CurrentMap.Height);
                }

                // set the loot's new position
                newLoot.Position = new Point(lootPosition % CurrentMap.Width, lootPosition / CurrentMap.Width);

                // add the Item to the MultiSpatialMap
                CurrentMap.Add(newLoot);
            }
        }
    }
}
