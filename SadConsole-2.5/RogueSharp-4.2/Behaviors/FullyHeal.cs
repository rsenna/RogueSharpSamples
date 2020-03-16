using RogueSharpSamples.LegacySadConsole.Core;
using RogueSharpSamples.LegacySadConsole.Interfaces;
using RogueSharpSamples.LegacySadConsole.Systems;

namespace RogueSharpSamples.LegacySadConsole.Behaviors
{
    public class FullyHeal : IBehavior
    {
        public bool Act(Monster monster, CommandSystem commandSystem)
        {
            if (monster.Health < monster.MaxHealth)
            {
                var healthToRecover = monster.MaxHealth - monster.Health;
                monster.Health = monster.MaxHealth;
                RogueGame.MessageLog.Add($"{monster.Name} catches his breath and recovers {healthToRecover} health");
                return true;
            }

            return false;
        }
    }
}
