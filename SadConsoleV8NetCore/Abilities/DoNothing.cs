using RogueSharpSamples.SadConsoleV8NetCore.Core;

namespace RogueSharpSamples.SadConsoleV8NetCore.Abilities
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
