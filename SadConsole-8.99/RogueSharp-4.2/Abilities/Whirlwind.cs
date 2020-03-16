using System.Collections.Generic;
using RogueSharp;
using RogueSharpSample2.Core;

namespace RogueSharpSample2.Abilities
{
    public class Whirlwind : Ability
    {
        public Whirlwind()
        {
            Name = "Whirlwind";
            TurnsToRefresh = 20;
            TurnsUntilRefreshed = 0;
        }

        protected override bool PerformAbility()
        {
            var map = RogueGame.DungeonMap;
            var player = RogueGame.Player;

            RogueGame.MessageLog.Add($"{player.Name} performs a whirlwind attack against all adjacent enemies");

            var monsterLocations = new List<Point>();

            foreach (var cell in map.GetCellsInCircle(player.X, player.Y, 1))
            foreach (var monsterLocation in map.GetMonsterLocations())
            {
                if (cell.X == monsterLocation.X && cell.Y == monsterLocation.Y)
                {
                    monsterLocations.Add(monsterLocation);
                }
            }

            foreach (var monsterLocation in monsterLocations)
            {
                var monster = map.GetMonsterAt(monsterLocation.X, monsterLocation.Y);
                if (monster != null)
                {
                    RogueGame.CommandSystem.Attack(player, monster);
                }
            }

            return true;
        }
    }
}
