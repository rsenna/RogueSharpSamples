﻿using RogueSharp;
using RogueSharp.Random;
using RogueSharpSample2.Core;

namespace RogueSharpSample2.Items
{
    public class DestructionWand : Item
    {
        public DestructionWand()
        {
            Name = "Wand of Destruction";
            RemainingUses = 3;
        }

        protected override bool UseItem()
        {
            var map = RogueGame.DungeonMap;
            var player = RogueGame.Player;
            var edgePoint = GetRandomEdgePoint(map);

            RogueGame.MessageLog.Add($"{player.Name} uses a {Name} and chaotically unleashes a void beam");
            var voidAttackActor = new Actor
            {
                Attack = 6,
                AttackChance = 90,
                Name = "The Void"
            };
            ICell previousCell = null;
            foreach (var cell in map.GetCellsAlongLine(player.X, player.Y, edgePoint.X, edgePoint.Y))
            {
                if (cell.X == player.X && cell.Y == player.Y)
                {
                    continue;
                }

                var monster = map.GetMonsterAt(cell.X, cell.Y);
                if (monster != null)
                {
                    RogueGame.CommandSystem.Attack(voidAttackActor, monster);
                }
                else
                {
                    map.SetCellProperties(cell.X, cell.Y, true, true, true);
                    if (previousCell != null)
                    {
                        if (cell.X != previousCell.X || cell.Y != previousCell.Y)
                        {
                            map.SetCellProperties(cell.X + 1, cell.Y, true, true, true);
                        }
                    }

                    previousCell = cell;
                }
            }

            RemainingUses--;

            return true;
        }

        private Point GetRandomEdgePoint(DungeonMap map)
        {
            var random = new DotNetRandom();
            var result = random.Next(1, 4);
            switch (result)
            {
                case 1: // TOP
                {
                    return new Point(random.Next(3, map.Width - 3), 3);
                }
                case 2: // BOTTOM
                {
                    return new Point(random.Next(3, map.Width - 3), map.Height - 3);
                }
                case 3: // RIGHT
                {
                    return new Point(map.Width - 3, random.Next(3, map.Height - 3));
                }
                case 4: // LEFT
                {
                    return new Point(3, random.Next(3, map.Height - 3));
                }
                default:
                {
                    return new Point(3, 3);
                }
            }
        }
    }
}
