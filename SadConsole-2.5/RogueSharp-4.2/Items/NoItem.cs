﻿using RogueSharpSamples.LegacySadConsole.Core;

namespace RogueSharpSamples.LegacySadConsole.Items
{
    public class NoItem : Item
    {
        public NoItem()
        {
            Name = "None";
            RemainingUses = 1;
        }

        protected override bool UseItem()
        {
            return false;
        }
    }
}
