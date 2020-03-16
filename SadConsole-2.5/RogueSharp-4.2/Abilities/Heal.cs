using System;
using RogueSharpSamples.LegacySadConsole.Core;

namespace RogueSharpSamples.LegacySadConsole.Abilities
{
    public class Heal : Ability
    {
        private readonly int _amountToHeal;

        public Heal(int amountToHeal)
        {
            Name = "Heal Self";
            TurnsToRefresh = 60;
            TurnsUntilRefreshed = 0;
            _amountToHeal = amountToHeal;
        }

        protected override bool PerformAbility()
        {
            var player = RogueGame.Player;

            player.Health = Math.Min(player.MaxHealth, player.Health + _amountToHeal);

            return true;
        }
    }
}
