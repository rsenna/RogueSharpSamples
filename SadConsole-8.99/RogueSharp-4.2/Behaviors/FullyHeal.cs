using RogueSharpSample2.Core;
using RogueSharpSample2.Interfaces;
using RogueSharpSample2.Systems;

namespace RogueSharpSample2.Behaviors
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
