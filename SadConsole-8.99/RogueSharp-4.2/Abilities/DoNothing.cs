﻿using RogueSharpSample2.Core;

namespace RogueSharpSample2.Abilities
{
    public class DoNothing : Ability
    {
        public DoNothing()
        {
            Name = "None";
            TurnsToRefresh = 0;
            TurnsUntilRefreshed = 0;
        }

        protected override bool PerformAbility()
        {
            RogueGame.MessageLog.Add("No ability in that slot");
            return false;
        }
    }
}
