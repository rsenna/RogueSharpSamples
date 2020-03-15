using RogueSharp;
using RogueSharpSamples.LegacySadConsole.Core;
using RogueSharpSamples.LegacySadConsole.Interfaces;

namespace RogueSharpSamples.LegacySadConsole.Abilities
{
    public class Fireball : Ability, ITargetable
    {
        private readonly int _area;
        private readonly int _attack;
        private readonly int _attackChance;

        public Fireball(int attack, int attackChance, int area)
        {
            Name = "Fireball";
            TurnsToRefresh = 40;
            TurnsUntilRefreshed = 0;
            _attack = attack;
            _attackChance = attackChance;
            _area = area;
        }

        public void SelectTarget(Point target)
        {
            var map = RogueGame.DungeonMap;
            var player = RogueGame.Player;
            RogueGame.MessageLog.Add($"{player.Name} casts a {Name}");
            var fireballActor = new Actor
            {
                Attack = _attack,
                AttackChance = _attackChance,
                Name = Name
            };
            foreach (var cell in map.GetCellsInCircle(target.X, target.Y, _area))
            {
                var monster = map.GetMonsterAt(cell.X, cell.Y);
                if (monster != null)
                {
                    RogueGame.CommandSystem.Attack(fireballActor, monster);
                }
            }
        }

        protected override bool PerformAbility()
        {
            return RogueGame.TargetingSystem.SelectArea(this, _area);
        }
    }
}
