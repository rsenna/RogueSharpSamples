using RogueSharpSample1.Core;
using RogueSharpSample1.Interfaces;
using RogueSharpSample1.Systems;

namespace RogueSharpSample1.Behaviors
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
