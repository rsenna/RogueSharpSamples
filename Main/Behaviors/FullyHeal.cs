using RogueSharp.SadConsole.Playground.Main.Core;
using RogueSharp.SadConsole.Playground.Main.Interfaces;
using RogueSharp.SadConsole.Playground.Main.Systems;

namespace RogueSharp.SadConsole.Playground.Main.Behaviors
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
