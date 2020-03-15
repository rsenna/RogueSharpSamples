using RogueSharp.SadConsole.Playground.Main.Core;

namespace RogueSharp.SadConsole.Playground.Main.Abilities
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
