using RogueSharp;
using RogueSharpSample1.Core;
using RogueSharpSample1.Interfaces;

namespace RogueSharpSample1.Abilities
{
    public class LightningBolt : Ability, ITargetable
    {
        private readonly int _attack;
        private readonly int _attackChance;

        public LightningBolt(int attack, int attackChance)
        {
            Name = "Lightning Bolt";
            TurnsToRefresh = 40;
            TurnsUntilRefreshed = 0;
            _attack = attack;
            _attackChance = attackChance;
        }

        public void SelectTarget(Point target)
        {
            var map = RogueGame.DungeonMap;
            var player = RogueGame.Player;
            RogueGame.MessageLog.Add($"{player.Name} casts a {Name}");

            var lightningBoltActor = new Actor
            {
                Attack = _attack,
                AttackChance = _attackChance,
                Name = Name
            };
            foreach (var cell in map.GetCellsAlongLine(player.X, player.Y, target.X, target.Y))
            {
                if (cell.IsWalkable)
                {
                    continue;
                }

                if (cell.X == player.X && cell.Y == player.Y)
                {
                    continue;
                }

                var monster = map.GetMonsterAt(cell.X, cell.Y);
                if (monster != null)
                {
                    RogueGame.CommandSystem.Attack(lightningBoltActor, monster);
                }
                else
                    // We hit a wall so stop the bolt
                    // TODO: consider having bolts and fireballs destroy walls and leave rubble
                {
                    return;
                }
            }
        }

        protected override bool PerformAbility()
        {
            return RogueGame.TargetingSystem.SelectLine(this);
        }
    }
}
