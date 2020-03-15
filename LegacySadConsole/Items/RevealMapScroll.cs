﻿using RogueSharpSamples.LegacySadConsole.Core;

namespace RogueSharpSamples.LegacySadConsole.Items
{
    public class RevealMapScroll : Item
    {
        public RevealMapScroll()
        {
            Name = "Magic Map";
            RemainingUses = 1;
        }

        protected override bool UseItem()
        {
            var map = RogueGame.DungeonMap;

            RogueGame.MessageLog.Add(
                $"{RogueGame.Player.Name} reads a {Name} and gains knowledge of the surrounding area");

            foreach (var cell in map.GetAllCells())
            {
                if (cell.IsWalkable)
                {
                    map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }

            RemainingUses--;

            return true;
        }
    }
}
