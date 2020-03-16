using RogueSharpSamples.SadConsoleV8NetCore.Core;
using RogueSharpSamples.SadConsoleV8NetCore.Interfaces;
using RogueSharpSamples.SadConsoleV8NetCore.Systems;

namespace RogueSharpSamples.SadConsoleV8NetCore.Behaviors
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
