using RogueSharp;
using RogueSharpSample2.Core;
using RogueSharpSample2.Interfaces;

namespace RogueSharpSample2.Abilities
{
    public class MagicMissile : Ability, ITargetable
    {
        private readonly int _attack;
        private readonly int _attackChance;

        public MagicMissile(int attack, int attackChance)
        {
            Name = "Magic Missile";
            TurnsToRefresh = 10;
            TurnsUntilRefreshed = 0;
            _attack = attack;
            _attackChance = attackChance;
        }

        public void SelectTarget(Point target)
        {
            var map = RogueGame.DungeonMap;
            var player = RogueGame.Player;
            var monster = map.GetMonsterAt(target.X, target.Y);
            if (monster != null)
            {
                RogueGame.MessageLog.Add($"{player.Name} casts a {Name} at {monster.Name}");
                var magicMissleActor = new Actor
                {
                    Attack = _attack, AttackChance = _attackChance, Name = Name
                };
                RogueGame.CommandSystem.Attack(magicMissleActor, monster);
            }
        }

        protected override bool PerformAbility()
        {
            return RogueGame.TargetingSystem.SelectMonster(this);
        }
    }
}
