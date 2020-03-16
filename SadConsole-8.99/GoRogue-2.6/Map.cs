using System.Linq;
using GoRogue;
using GoRogueSample3.Entities;
using GoRogueSample3.Tiles;
using Microsoft.Xna.Framework;
using SadConsole;

namespace GoRogueSample3
{
    // Stores, manipulates and queries Tile data
    public class Map
    {
        public static readonly IDGenerator IdGenerator = new IDGenerator(); // A static IDGenerator that all Entities can access

        //Build a new map with a specified width and height
        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new TileBase[width * height];
            Entities = new MultiSpatialMap<Entity>();
        }

        public MultiSpatialMap<Entity> Entities { get; } // Keeps track of all the Entities on the map

        public TileBase[] Tiles { get; }

        public int Width { get; }

        public int Height { get; set; }

        // IsTileWalkable checks
        // to see if the actor has tried
        // to walk off the map or into a non-walkable tile
        // Returns true if the tile location is walkable
        // false if tile location is not walkable or is off-map
        public bool IsTileWalkable(Point location)
        {
            // first make sure that actor isn't trying to move
            // off the limits of the map
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
            {
                return false;
            }

            // then return whether the tile is walkable
            return !Tiles[location.Y * Width + location.X].IsBlockingMove;
        }

        //really snazzy way of checking whether a certain type of
        //tile is at a specified location in the map's Tiles
        //and if it exists, return that Tile
        //accepts an x/y coordinate
        public T GetTileAt<T>(int x, int y)
            where T : TileBase
        {
            var locationIndex = Helpers.GetIndexFromPoint(x, y, Width);
            // make sure the index is within the boundaries of the map!
            if (locationIndex <= Width * Height && locationIndex >= 0)
            {
                if (Tiles[locationIndex] is T)
                {
                    return (T) Tiles[locationIndex];
                }

                return null;
            }

            return null;
        }

        // Checking whether a certain type of
        // entity is at a specified location the manager's list of entities
        // and if it exists, return that Entity
        public T GetEntityAt<T>(Point location)
            where T : Entity => Entities.GetItems(location).OfType<T>().FirstOrDefault();

        // Removes an Entity from the MultiSpatialMap
        public void Remove(Entity entity)
        {
            // remove from SpatialMap
            Entities.Remove(entity);

            // Link up the entity's Moved event to a new handler
            entity.Moved -= OnEntityMoved;
        }

        // Adds an Entity to the MultiSpatialMap
        public void Add(Entity entity)
        {
            // add entity to the SpatialMap
            Entities.Add(entity, entity.Position);

            // Link up the entity's Moved event to a new handler
            entity.Moved += OnEntityMoved;
        }

        // When the Entity's .Moved value changes, it triggers this event handler
        // which updates the Entity's current position in the SpatialMap
        private void OnEntityMoved(object sender, SadConsole.Entities.Entity.EntityMovedEventArgs args)
        {
            Entities.Move(args.Entity as Entity, args.Entity.Position);
        }
    }
}
